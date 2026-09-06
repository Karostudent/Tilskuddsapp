namespace Tilskuddsapp.Models;

public class GrantApplication
{
    public int Id { get; set; }
    public int MunicipalityId { get; set; }
    public int DoctorId { get; set; }
    public int GrantRoundId { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
    public GrantType? GrantType { get; set; }

    public bool ConfirmsSpecialization { get; set; }
    public DateOnly? SpecializationStartDate { get; set; }
    public DateOnly? ExpectedCompletionDate { get; set; }

    public bool ConfirmsAlisAgreement { get; set; }
    public bool ConfirmsOfficialAgreementTemplate { get; set; }
    public bool ConfirmsNoCommercialAgencyAffiliation { get; set; }
    public DateOnly? AgreementEffectiveFrom { get; set; }

    public List<EmploymentPeriod> EmploymentPeriods { get; set; } = [];
    public Reimbursement Reimbursement { get; set; } = new();

    public string CreatedByUserId { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SubmittedAt { get; set; }
}
