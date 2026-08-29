using Tilskuddsapp.Models;
using static Tilskuddsapp.Models.Enums;

namespace Tilskuddsapp.Services
{
    public class GrantCalculationService
    : IGrantCalculationService
    {
        public GrantCalculation Calculate(GrantCase grantCase)
        {
            var result = new GrantCalculation();

            result.TotalExpenses =
                grantCase.Expenses.Sum(x => x.Amount);

            // Veileder
            if (grantCase.Supervisor != null)
            {
                result.SupervisorAmount =
                    grantCase.SupervisionHours *
                    grantCase.Supervisor.HourlyRate;
            }

            if (grantCase.Doctor.DoctorType ==
                DoctorType.SelvstendigNæringsdrivende)
            {
                // Beregn selvstendig næringsdrivende
            }
            else
            {
                // Beregn legevakt
            }

            // Kontroller totalsum
            // Legg til warnings

            return result;
        }
    }
}
