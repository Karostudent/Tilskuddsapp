using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Tilskuddsapp.ViewModels
{
    public class GrantCaseFormViewModel
    {
        [Required(ErrorMessage = "Navn på lege må fylles ut.")]
        public string DoctorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Du må velge type lege.")]
        public string DoctorType { get; set; } = string.Empty;

        [Range(2020, 2100, ErrorMessage = "Ugyldig år.")]
        public int Year { get; set; }

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Innvilget tilskudd må være større enn 0.")]
        public decimal ApprovedGrant { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DocumentedExpenses { get; set; }

        public string SupervisorName { get; set; } = string.Empty;

        public string SupervisorType { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SupervisionHours { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SupervisorHourlyRate { get; set; }

        public decimal SupervisorAmount { get; set; }

        public decimal DoctorAmount { get; set; }

        public decimal PracticeAmount { get; set; }

        public decimal TotalAllocated { get; set; }

        public string? Notes { get; set; }

        // Advarsler og meldinger
        public List<string> Warnings { get; set; } = new List<string>();

        public IEnumerable<SelectListItem> Doctors { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> Supervisors { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}