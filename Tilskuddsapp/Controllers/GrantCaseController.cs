using Microsoft.AspNetCore.Mvc;
using Tilskuddsapp.Models;
using Tilskuddsapp.ViewModels;
using static Tilskuddsapp.Models.Enums;

namespace Tilskuddsapp.Controllers
{
    public class GrantCasesController : Controller
    {
        public IActionResult GrantCaseIndex()
        {
            var grantCases = GetTestGrantCases();

            return View(grantCases);
        }

        public IActionResult GrantCaseDetails(int id)
        {
            var grantCases = GetTestGrantCases();

            var grantCase =
                grantCases.FirstOrDefault(x => x.GrantCaseId == id);

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
        public IActionResult Create(GrantCaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("GrantCaseCreate", model);
            }

            CalculateGrant(model);

            return View("GrantCaseCalculationResult", model);
        }

        private void CalculateGrant(GrantCaseFormViewModel model)
        {
            // Beregn honorar til veileder
            model.SupervisorAmount =
                model.SupervisionHours *
                model.SupervisorHourlyRate;

            // Veileder skal ikke kunne få mer enn innvilget tilskudd
            if (model.SupervisorAmount > model.ApprovedGrant)
            {
                model.SupervisorAmount =
                    model.ApprovedGrant;
            }

            var remainingGrant =
                model.ApprovedGrant -
                model.SupervisorAmount;

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

        private List<GrantCase> GetTestGrantCases()
        {
            return new List<GrantCase>
            {
                new GrantCase
                {
                    GrantCaseId = 1,
                    DoctorName = "Kari Nordmann",
                    Year = 2026,
                    ApprovedGrant = 120000,
                    GrantCaseStatus =
                        GrantCaseStatus.UnderBehandling
                },

                new GrantCase
                {
                    GrantCaseId = 2,
                    DoctorName = "Ola Hansen",
                    Year = 2026,
                    ApprovedGrant = 85000,
                    GrantCaseStatus =
                        GrantCaseStatus.Godkjent
                }
            };
        }
    }
}