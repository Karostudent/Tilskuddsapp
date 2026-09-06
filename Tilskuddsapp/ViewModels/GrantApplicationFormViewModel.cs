using Tilskuddsapp.Models;

namespace Tilskuddsapp.ViewModels;

// Editable form data is kept separate from persistence and authenticated identity.
// Draft fields remain nullable. Submission validation belongs in a dedicated service.
public class GrantApplicationFormViewModel
{
    public SupervisionCertificateViewModel Certificate { get; set; } = new();
    public int? Id { get; set; }
    public int Version { get; set; }
    public string HprNumber { get; set; } = "";

    // Populate these from a trusted lookup when HPR integration is implemented.
    public string? DoctorName { get; set; }
    public List<string> DoctorProfessions { get; set; } = [];

    public GrantType? GrantType { get; set; }
    public bool ConfirmsSpecialization { get; set; }
    public DateOnly? SpecializationStartDate { get; set; }
    public DateOnly? ExpectedCompletionDate { get; set; }

    public bool ConfirmsAlisAgreement { get; set; }
    public bool ConfirmsOfficialAgreementTemplate { get; set; }
    public bool ConfirmsNoCommercialAgencyAffiliation { get; set; }
    public DateOnly? AgreementEffectiveFrom { get; set; }

    // UI selections must be reconciled with employment rows before persistence.
    public List<PositionType> SelectedPositionTypes { get; set; } = [];
    public List<EmploymentPeriodInputModel> EmploymentPeriods { get; set; } = [];

    public decimal? AbsenceCompensation { get; set; }
    public decimal? LearningActivityExpenses { get; set; }
    public decimal? SupervisionExpenses { get; set; }
    public bool? HasAdditionalSupervisionCosts { get; set; }
    public decimal? AdditionalSupervisionCosts { get; set; }
}
