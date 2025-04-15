using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOutAdminV2.Enum
{
    public enum ESanctionDuration
    {
        TwoWeeks,      // 2 semaines
        OneMonth,      // 1 mois
        SixMonths,     // 6 mois
        OneYear,       // 1 an
        Permanent      // Permanent
    }

    public static class SanctionDurationExtensions
    {
        public static string GetDisplayName(this ESanctionDuration duration)
        {
            return duration switch
            {
                ESanctionDuration.TwoWeeks => "2 semaines",
                ESanctionDuration.OneMonth => "1 mois",
                ESanctionDuration.SixMonths => "6 mois",
                ESanctionDuration.OneYear => "1 an",
                ESanctionDuration.Permanent => "Permanent",
                _ => duration.ToString()
            };
        }

        public static DateTime? CalculateEndDate(this ESanctionDuration duration, DateTime startDate)
        {
            return duration switch
            {
                ESanctionDuration.TwoWeeks => startDate.AddDays(14),
                ESanctionDuration.OneMonth => startDate.AddMonths(1),
                ESanctionDuration.SixMonths => startDate.AddMonths(6),
                ESanctionDuration.OneYear => startDate.AddYears(1),
                ESanctionDuration.Permanent => null, // Aucune date de fin pour les bannissements permanents
                _ => null
            };
        }
    }
}