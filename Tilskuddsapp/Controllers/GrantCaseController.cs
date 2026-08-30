using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tilskuddsapp.Data;
using Tilskuddsapp.Models;
using Tilskuddsapp.ViewModels;
using static Tilskuddsapp.Models.Enums;

namespace Tilskuddsapp.Controllers
{
    public class GrantCasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GrantCasesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GrantCaseIndex()
        {
            var grantCases = await _context.GrantCases
                .Include(g => g.Doctor)
                .Include(g => g.Supervisor)
                .OrderByDescending(g => g.Year)
                .ThenBy(g => g.DoctorName)
                .ToListAsync();

            return View(grantCases);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAllToExcel()
        {
            var grantCases = await _context.GrantCases
                .Include(g => g.Doctor)
                .Include(g => g.Supervisor)
                .OrderByDescending(g => g.Year)
                .ThenBy(g => g.DoctorName)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tilskuddssaker");

            // Overskrifter
            worksheet.Cell(1, 1).Value = "År";
            worksheet.Cell(1, 2).Value = "Lege";
            worksheet.Cell(1, 3).Value = "Legetype";
            worksheet.Cell(1, 4).Value = "Veileder";
            worksheet.Cell(1, 5).Value = "Veiledertype";
            worksheet.Cell(1, 6).Value = "Innvilget tilskudd";
            worksheet.Cell(1, 7).Value = "Maksimumsbeløp";
            worksheet.Cell(1, 8).Value = "Veiledningstimer";
            worksheet.Cell(1, 9).Value = "Status";
            worksheet.Cell(1, 10).Value = "Merknader";

            // Style på overskrifter
            var headerRange = worksheet.Range(1, 1, 1, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data
            int row = 2;
            foreach (var grantCase in grantCases)
            {
                worksheet.Cell(row, 1).Value = grantCase.Year;
                worksheet.Cell(row, 2).Value = grantCase.DoctorName;
                worksheet.Cell(row, 3).Value = grantCase.Doctor?.DoctorType.ToString() ?? "";
                worksheet.Cell(row, 4).Value = grantCase.Supervisor?.Name ?? "";
                worksheet.Cell(row, 5).Value = grantCase.Supervisor?.SupervisorType.ToString() ?? "";
                worksheet.Cell(row, 6).Value = grantCase.ApprovedGrant;
                worksheet.Cell(row, 7).Value = grantCase.MaximumGrant ?? 0;
                worksheet.Cell(row, 8).Value = grantCase.SupervisionHours;
                worksheet.Cell(row, 9).Value = grantCase.GrantCaseStatus.ToString();
                worksheet.Cell(row, 10).Value = grantCase.Notes ?? "";

                row++;
            }

            // Autofit kolonner
            worksheet.Columns().AdjustToContents();

            // Returner Excel-fil
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Tilskuddssaker_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> GrantCaseDetails(int id)
        {
            var grantCase = await _context.GrantCases
                .Include(g => g.Doctor)
                .Include(g => g.Supervisor)
                .FirstOrDefaultAsync(g => g.GrantCaseId == id);

            if (grantCase == null)
            {
                return NotFound();
            }

            return View(grantCase);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new GrantCaseFormViewModel
            {
                Year = DateTime.Now.Year,
                SupervisionHours = 57.75m
            };

            return View("GrantCaseCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GrantCaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("GrantCaseCreate", model);
            }

            CalculateGrant(model);

            // Valider at total fordeling ikke overskrider innvilget tilskudd
            if (model.TotalAllocated > model.ApprovedGrant)
            {
                ModelState.AddModelError("",
                    $"Total fordeling ({model.TotalAllocated:N0} kr) overskrider innvilget tilskudd ({model.ApprovedGrant:N0} kr).");
                return View("GrantCaseCreate", model);
            }

            // Sjekk om Doctor allerede eksisterer (basert på navn)
            var existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Name == model.DoctorName);

            Doctor doctor;
            if (existingDoctor != null)
            {
                doctor = existingDoctor;
            }
            else
            {
                // Opprett ny Doctor
                doctor = new Doctor
                {
                    Name = model.DoctorName,
                    DoctorType = model.DoctorType == "SelfEmployed"
                        ? DoctorType.SelvstendigNæringsdrivende
                        : DoctorType.Legevakt,
                    Active = true
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync(); // Få DoctorId
            }

            // Lagre DoctorId for senere bruk
            TempData["DoctorId"] = doctor.DoctorId;
            TempData["DoctorName"] = doctor.Name;

            return View("GrantCaseCalculationResult", model);
        }

        private void CalculateGrant(GrantCaseFormViewModel model)
        {
            // Tøm tidligere advarsler
            model.Warnings.Clear();

            // Beregn honorar til veileder
            model.SupervisorAmount =
                model.SupervisionHours *
                model.SupervisorHourlyRate;

            // Veileder skal ikke kunne få mer enn innvilget tilskudd
            if (model.SupervisorAmount > model.ApprovedGrant)
            {
                model.SupervisorAmount =
                    model.ApprovedGrant;
                model.Warnings.Add("⚠️ Veilederhonorar overstiger innvilget tilskudd og er justert ned.");
            }

            var remainingGrant =
                model.ApprovedGrant -
                model.SupervisorAmount;

            // Advarsel for legevakt med ekstern veileder
            if (model.DoctorType == "EmergencyClinic" && model.SupervisorType == "External")
            {
                model.Warnings.Add("⚠️ Legevakt-lege bruker ekstern veileder. Dette er ugunstig da veilederhonorar går til ekstern veileder i stedet for legevakta.");

                // Beregn hvor mye som "spises opp"
                var percentageUsedBySupervisor = (model.SupervisorAmount / model.ApprovedGrant) * 100;
                model.Warnings.Add($"💡 Veilederhonorar utgjør {percentageUsedBySupervisor:N1}% av innvilget tilskudd.");
            }

            if (model.DoctorType == "SelfEmployed")
            {
                model.DoctorAmount =
                    Math.Min(
                        model.DocumentedExpenses,
                        remainingGrant);

                model.PracticeAmount =
                    remainingGrant -
                    model.DoctorAmount;
            }
            else if (model.DoctorType == "EmergencyClinic")
            {
                model.DoctorAmount = 0;

                model.PracticeAmount =
                    remainingGrant;
            }

            model.TotalAllocated =
                model.DoctorAmount +
                model.SupervisorAmount +
                model.PracticeAmount;
        }

        [HttpPost]
        public async Task<IActionResult> SaveGrantCase(GrantCaseFormViewModel model)
        {
            // Hent DoctorId fra TempData (lagret i Create-metoden)
            var doctorId = TempData["DoctorId"] as int?;

            if (doctorId == null)
            {
                ModelState.AddModelError("", "Feil: Doctor ikke funnet. Vennligst start på nytt.");
                return View("GrantCaseCreate", model);
            }

            // Sjekk om Supervisor allerede eksisterer (basert på navn)
            Supervisor? supervisor = null;
            if (!string.IsNullOrWhiteSpace(model.SupervisorName))
            {
                var existingSupervisor = await _context.Supervisors
                    .FirstOrDefaultAsync(s => s.Name == model.SupervisorName);

                if (existingSupervisor != null)
                {
                    supervisor = existingSupervisor;
                }
                else
                {
                    // Opprett ny Supervisor
                    supervisor = new Supervisor
                    {
                        Name = model.SupervisorName,
                        SupervisorType = model.SupervisorType == "Internal"
                            ? SupervisorType.Intern
                            : SupervisorType.Ekstern,
                        HourlyRate = model.SupervisorHourlyRate,
                        Active = true
                    };
                    _context.Supervisors.Add(supervisor);
                    await _context.SaveChangesAsync(); // Få SupervisorId
                }
            }

            // Opprett GrantCase
            var grantCase = new GrantCase
            {
                DoctorId = doctorId.Value,
                DoctorName = model.DoctorName,
                SupervisorId = supervisor?.SupervisorId,
                Year = model.Year,
                ApprovedGrant = model.ApprovedGrant,
                MaximumGrant = model.ApprovedGrant, // Kan justeres senere
                SupervisionHours = model.SupervisionHours,
                GrantCaseStatus = GrantCaseStatus.UnderBehandling,
                Notes = model.Notes
            };

            _context.GrantCases.Add(grantCase);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Tilskuddssak for {model.DoctorName} er lagret!";

            return RedirectToAction("GrantCaseIndex");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int id)
        {
            var grantCase = await _context.GrantCases
                .Include(g => g.Doctor)
                .Include(g => g.Supervisor)
                .FirstOrDefaultAsync(g => g.GrantCaseId == id);

            if (grantCase == null)
            {
                return NotFound();
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Samlet E-bilag");

            // Header
            worksheet.Cell(1, 1).Value = "Samlet E-bilag for tilskudd";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;

            // Lege informasjon
            int row = 3;
            worksheet.Cell(row, 1).Value = "Lege:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 2).Value = grantCase.Doctor?.Name ?? "Ukjent";
            row++;

            worksheet.Cell(row, 1).Value = "Legetype:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 2).Value = grantCase.Doctor?.DoctorType.ToString() ?? "Ukjent";
            row++;

            worksheet.Cell(row, 1).Value = "År:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 2).Value = grantCase.Year;
            row++;

            worksheet.Cell(row, 1).Value = "Godkjent tilskudd:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 2).Value = grantCase.ApprovedGrant;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00 kr";
            row += 2;

            // Fordeling header
            worksheet.Cell(row, 1).Value = "Fordeling av tilskudd";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 12;
            row++;

            // Table headers
            worksheet.Cell(row, 1).Value = "Post";
            worksheet.Cell(row, 2).Value = "Beskrivelse";
            worksheet.Cell(row, 3).Value = "Beløp";
            worksheet.Range(row, 1, row, 3).Style.Font.Bold = true;
            worksheet.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            // Veileder
            if (grantCase.Supervisor != null && grantCase.SupervisionHours > 0)
            {
                worksheet.Cell(row, 1).Value = "Veileder";
                worksheet.Cell(row, 2).Value = $"{grantCase.Supervisor.Name} ({grantCase.SupervisionHours} timer à {grantCase.Supervisor.HourlyRate} kr)";
                decimal supervisorAmount = grantCase.SupervisionHours * grantCase.Supervisor.HourlyRate;
                worksheet.Cell(row, 3).Value = supervisorAmount;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 kr";
                row++;
            }

            // Lege
            decimal doctorAmount = grantCase.ApprovedGrant - (grantCase.Supervisor != null && grantCase.SupervisionHours > 0
                ? grantCase.SupervisionHours * grantCase.Supervisor.HourlyRate
                : 0);
            worksheet.Cell(row, 1).Value = "Lege";
            worksheet.Cell(row, 2).Value = grantCase.Doctor?.Name ?? "Ukjent";
            worksheet.Cell(row, 3).Value = doctorAmount;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 kr";
            row++;

            // Total
            row++;
            worksheet.Cell(row, 1).Value = "Totalt utbetalt:";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 3).Value = grantCase.ApprovedGrant;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 kr";
            worksheet.Cell(row, 3).Style.Font.Bold = true;

            // Adjust column widths
            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 40;
            worksheet.Column(3).Width = 15;

            // Save to memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"E-bilag_{grantCase.Doctor?.Name?.Replace(" ", "_")}_{grantCase.Year}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}