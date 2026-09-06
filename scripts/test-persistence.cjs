const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const { chromium } = require(path.join(process.env.TEMP, 'tilskuddsapp-browser-tests/node_modules/playwright'));

(async () => {
    const browser = await chromium.launch({ channel: 'msedge', headless: true });
    let id;
    try {
        const page = await browser.newPage();
        const errors = [];
        page.on('pageerror', error => errors.push(error.message));
        await page.goto('http://localhost:5189/GrantApplications/Create');
        await page.locator('#HprNumber').fill('TEST-DO-NOT-USE');
        await page.locator('#ConfirmsSpecialization').check();
        await page.locator('#SpecializationStartDate').fill('2025-06-01');
        await page.locator('#grant-type-1').check();
        await page.locator('#ConfirmsAlisAgreement').check();
        await page.locator('#AgreementEffectiveFrom').fill('2025-06-01');
        await page.locator('#position-type-1').check();
        await page.locator('#add-employment').click();
        await page.locator('[name="EmploymentPeriods[0].PositionType"]').selectOption('1');
        await page.locator('[name="EmploymentPeriods[0].PositionPercentage"]').fill('80.5');
        await page.locator('[name="EmploymentPeriods[0].FundingFrom"]').fill('2025-06-01');
        await page.locator('#next-step').click();
        await page.locator('#Certificate_DoctorName').fill('Test Doctor');
        await page.locator('#Certificate_SupervisorName').fill('Test Supervisor');
        await page.locator('#session-date-0').fill('2026-09-01');
        await page.locator('[data-hours="1.5"]').click();
        await page.locator('#session-topic-0').fill('Persistence test <script>no code</script>');
        await page.locator('#add-supervision').click();
        await page.locator('#session-date-1').fill('2026-09-02');
        await page.locator('#session-hours-1').fill('2.25');
        await page.locator('#session-topic-1').fill('Second session');
        const responsePromise = page.waitForResponse(r => r.url().endsWith('/Save') && r.request().method() === 'POST');
        await page.locator('#certificate-step .save-draft').click();
        const response = await responsePromise;
        const result = await response.json();
        assert.equal(response.status(), 200, JSON.stringify(result));
        id = result.id;
        await page.waitForFunction(() => document.querySelector('[name="Id"]').value !== '');
        await page.reload();
        assert.equal(await page.locator('#HprNumber').inputValue(), 'TEST-DO-NOT-USE');
        assert.equal(await page.locator('#ConfirmsSpecialization').isChecked(), true);
        assert.equal(await page.locator('[name="EmploymentPeriods[0].PositionPercentage"]').inputValue(), '80.5');
        await page.locator('#next-step').click();
        assert.equal(await page.locator('#Certificate_DoctorName').inputValue(), 'Test Doctor');
        assert.equal(await page.locator('#session-topic-0').inputValue(), 'Persistence test <script>no code</script>');
        assert.equal(await page.locator('#total-supervision-hours').textContent(), '3,75');
        await page.locator('.supervision-session').first().locator('.remove-supervision').click();
        const updatedPromise = page.waitForResponse(r => r.url().endsWith('/Save'));
        await page.locator('#certificate-step .save-draft').click();
        assert.equal((await updatedPromise).status(), 200);
        await page.waitForFunction(() => document.querySelector('[name="Version"]').value === '2');
        // Stale updates must not overwrite the saved version.
        const stale = await page.evaluate(async id => {
            const token = document.querySelector('[name="__RequestVerificationToken"]').value;
            return (await fetch('/GrantApplications/Save', { method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': token },
                body: JSON.stringify({ id, version: 1, certificate: { sessions: [] }, employmentPeriods: [] }) })).status;
        }, id);
        assert.equal(stale, 409);
        const noToken = await page.request.post('http://localhost:5189/GrantApplications/Save', { data: {} });
        assert.equal(noToken.status(), 400);
        execFileSync('docker', ['compose', 'restart', 'postgres'], { stdio: 'pipe' });
        for (let attempt = 0; attempt < 20; attempt++) {
            try {
                const opened = await page.goto('http://localhost:5189/GrantApplications/Create?id=' + id);
                if (opened.status() === 200) break;
            } catch {}
            await new Promise(resolve => setTimeout(resolve, 500));
        }
        await page.locator('#next-step').click();
        assert.equal(await page.locator('.supervision-session').count(), 1);
        assert.equal(await page.locator('#session-hours-0').inputValue(), '2.25');
        assert.equal(await page.locator('#session-topic-0').inputValue(), 'Second session');
        assert.equal(await page.locator('#Certificate_SupervisorName').inputValue(), 'Test Supervisor');
        await page.setViewportSize({ width: 390, height: 844 });
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth), true);
        assert.deepEqual(errors, []);
        console.log('PASS: browser save/reopen, booleans/dates/decimals, both steps, row removal, totals, stale-update rejection, CSRF, database restart persistence and mobile width.');
    } finally {
        await browser.close();
        if (Number.isInteger(id)) {
            execFileSync('docker', ['compose', 'exec', '-T', 'postgres', 'psql', '-U', 'tilskuddsapp', '-d', 'tilskuddsapp',
                '-c', 'DELETE FROM grant_drafts WHERE id = ' + id + " AND document->>'hprNumber' = 'TEST-DO-NOT-USE'"], { stdio: 'pipe' });
        }
    }
})().catch(error => { console.error(error); process.exitCode = 1; });
