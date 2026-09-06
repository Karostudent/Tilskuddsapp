using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Tilskuddsapp.Data;
using Tilskuddsapp.ViewModels;

namespace Tilskuddsapp.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class GrantApplicationsController(GrantDraftStore store, ILogger<GrantApplicationsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await store.ListAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(int? id, CancellationToken ct)
    {
        var model = id.HasValue ? await store.GetAsync(id.Value, ct) : new GrantApplicationFormViewModel();
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> Save([FromBody] GrantApplicationFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Kontroller tall og datoer før du lagrer." });
        if (model.Certificate is null || model.Certificate.Sessions is null || model.EmploymentPeriods is null)
            return BadRequest(new { message = "Skjemaet mangler opplysninger." });
        if (model.Certificate.Sessions.Any(s => s.Hours is < 0 or > 24)
            || model.EmploymentPeriods.Any(p => p.PositionPercentage is < 0 or > 100)
            || model.AbsenceCompensation is < 0 || model.LearningActivityExpenses is < 0
            || model.SupervisionExpenses is < 0 || model.AdditionalSupervisionCosts is < 0)
            return BadRequest(new { message = "Kontroller timer, stillingsprosent og beløp." });
        try
        {
            var result = await store.SaveAsync(model, ct);
            if (result is null)
                return Conflict(new { message = "Utkastet er endret i en annen fane eller finnes ikke lenger. Åpne det på nytt før du lagrer." });
            return Ok(new { id = result.Value.Id, version = result.Value.Version, message = "Utkastet er lagret." });
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Could not save grant draft.");
            return StatusCode(503, new { message = "Kunne ikke lagre. Kontroller at databasen kjører og prøv igjen. Feltene dine er beholdt." });
        }
    }
}
