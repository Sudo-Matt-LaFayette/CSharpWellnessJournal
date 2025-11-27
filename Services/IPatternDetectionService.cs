using System.Collections.Generic;
using System.Threading.Tasks;
using DailyCheckInJournal.Models;

namespace DailyCheckInJournal.Services
{
    public interface IPatternDetectionService
    {
        Task<List<Pattern>> DetectPatternsAsync(List<CheckIn> checkIns);
        Task<List<Pattern>> GetActivePatternsAsync();
    }
}

