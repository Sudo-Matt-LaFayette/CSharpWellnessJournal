using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyCheckInJournal.Models;

namespace DailyCheckInJournal.Services
{
    public interface IDataService
    {
        Task<List<CheckIn>> GetAllCheckInsAsync();
        Task<CheckIn?> GetCheckInAsync(DateTime date);
        Task SaveCheckInAsync(CheckIn checkIn);
        Task<List<Goal>> GetAllGoalsAsync();
        Task SaveGoalAsync(Goal goal);
        Task DeleteGoalAsync(string goalId);
        Task<List<Habit>> GetAllHabitsAsync();
        Task SaveHabitAsync(Habit habit);
        Task DeleteHabitAsync(string habitId);
        Task<List<Pattern>> GetAllPatternsAsync();
        Task SavePatternAsync(Pattern pattern);
        Task<AppSettings> GetSettingsAsync();
        Task SaveSettingsAsync(AppSettings settings);
        Task<List<CheckIn>> GetCheckInsInRangeAsync(DateTime startDate, DateTime endDate);
    }
}

