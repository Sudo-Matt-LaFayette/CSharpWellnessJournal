using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DailyCheckInJournal.Models;
using Newtonsoft.Json;

namespace DailyCheckInJournal.Services
{
    public class DataService : IDataService
    {
        private readonly string _dataDirectory;
        private readonly string _checkInsFile;
        private readonly string _goalsFile;
        private readonly string _habitsFile;
        private readonly string _patternsFile;
        private readonly string _settingsFile;
        private readonly ILoggerService? _logger;

        public DataService(ILoggerService? logger = null)
        {
            _logger = logger;
            try
            {
                _logger?.LogDebug("Initializing DataService...");
                _dataDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DailyCheckInJournal"
                );
                Directory.CreateDirectory(_dataDirectory);
                _logger?.LogDebug($"Data directory: {_dataDirectory}");

                _checkInsFile = Path.Combine(_dataDirectory, "checkins.json");
                _goalsFile = Path.Combine(_dataDirectory, "goals.json");
                _habitsFile = Path.Combine(_dataDirectory, "habits.json");
                _patternsFile = Path.Combine(_dataDirectory, "patterns.json");
                _settingsFile = Path.Combine(_dataDirectory, "settings.json");
                _logger?.LogInformation("DataService initialized successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing DataService");
                throw;
            }
        }

        public async Task<List<CheckIn>> GetAllCheckInsAsync()
        {
            try
            {
                _logger?.LogDebug("Loading all check-ins...");
                if (!File.Exists(_checkInsFile))
                {
                    _logger?.LogDebug("Check-ins file does not exist, returning empty list");
                    return new List<CheckIn>();
                }

                var json = await File.ReadAllTextAsync(_checkInsFile);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger?.LogDebug("Check-ins file is empty, returning empty list");
                    return new List<CheckIn>();
                }

                var checkIns = JsonConvert.DeserializeObject<List<CheckIn>>(json) ?? new List<CheckIn>();
                _logger?.LogDebug($"Loaded {checkIns.Count} check-ins");
                return checkIns;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading check-ins");
                throw;
            }
        }

        public async Task<CheckIn?> GetCheckInAsync(DateTime date)
        {
            var checkIns = await GetAllCheckInsAsync();
            var dateOnly = date.Date;
            return checkIns.FirstOrDefault(c => c.Date.Date == dateOnly);
        }

        public async Task SaveCheckInAsync(CheckIn checkIn)
        {
            try
            {
                _logger?.LogDebug($"Saving check-in for date: {checkIn.Date:yyyy-MM-dd}");
                var checkIns = await GetAllCheckInsAsync();
                var existing = checkIns.FirstOrDefault(c => c.Date.Date == checkIn.Date.Date);
                
                if (existing != null)
                {
                    _logger?.LogDebug("Updating existing check-in");
                    checkIns.Remove(existing);
                }
                else
                {
                    _logger?.LogDebug("Creating new check-in");
                }
                
                checkIns.Add(checkIn);
                checkIns = checkIns.OrderByDescending(c => c.Date).ToList();

                var json = JsonConvert.SerializeObject(checkIns, Formatting.Indented);
                await File.WriteAllTextAsync(_checkInsFile, json);
                _logger?.LogInformation($"Check-in saved successfully for {checkIn.Date:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error saving check-in for {checkIn.Date:yyyy-MM-dd}");
                throw;
            }
        }

        public async Task<List<Goal>> GetAllGoalsAsync()
        {
            if (!File.Exists(_goalsFile))
                return new List<Goal>();

            var json = await File.ReadAllTextAsync(_goalsFile);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Goal>();

            return JsonConvert.DeserializeObject<Goal[]>(json)?.ToList() ?? new List<Goal>();
        }

        public async Task SaveGoalAsync(Goal goal)
        {
            var goals = await GetAllGoalsAsync();
            var existing = goals.FirstOrDefault(g => g.Id == goal.Id);
            
            if (existing != null)
            {
                goals.Remove(existing);
            }
            
            goals.Add(goal);

            var json = JsonConvert.SerializeObject(goals, Formatting.Indented);
            await File.WriteAllTextAsync(_goalsFile, json);
        }

        public async Task DeleteGoalAsync(string goalId)
        {
            var goals = await GetAllGoalsAsync();
            goals.RemoveAll(g => g.Id == goalId);

            var json = JsonConvert.SerializeObject(goals, Formatting.Indented);
            await File.WriteAllTextAsync(_goalsFile, json);
        }

        public async Task<List<Habit>> GetAllHabitsAsync()
        {
            if (!File.Exists(_habitsFile))
            {
                // Initialize with default habits
                var defaultHabits = new List<Habit>
                {
                    new Habit { Name = "Exercise", Category = HabitCategory.Exercise, IsCustom = false },
                    new Habit { Name = "Eat Breakfast", Category = HabitCategory.Nutrition, IsCustom = false },
                    new Habit { Name = "Eat Lunch", Category = HabitCategory.Nutrition, IsCustom = false },
                    new Habit { Name = "Eat Dinner", Category = HabitCategory.Nutrition, IsCustom = false },
                    new Habit { Name = "Take Breaks", Category = HabitCategory.SelfCare, IsCustom = false },
                    new Habit { Name = "Drink Water", Category = HabitCategory.Nutrition, IsCustom = false }
                };
                await SaveHabitsAsync(defaultHabits);
                return defaultHabits;
            }

            var json = await File.ReadAllTextAsync(_habitsFile);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Habit>();

            return JsonConvert.DeserializeObject<List<Habit>>(json) ?? new List<Habit>();
        }

        private async Task SaveHabitsAsync(List<Habit> habits)
        {
            var json = JsonConvert.SerializeObject(habits, Formatting.Indented);
            await File.WriteAllTextAsync(_habitsFile, json);
        }

        public async Task SaveHabitAsync(Habit habit)
        {
            var habits = await GetAllHabitsAsync();
            var existing = habits.FirstOrDefault(h => h.Id == habit.Id);
            
            if (existing != null)
            {
                habits.Remove(existing);
            }
            
            habits.Add(habit);

            var json = JsonConvert.SerializeObject(habits, Formatting.Indented);
            await File.WriteAllTextAsync(_habitsFile, json);
        }

        public async Task DeleteHabitAsync(string habitId)
        {
            var habits = await GetAllHabitsAsync();
            habits.RemoveAll(h => h.Id == habitId);

            var json = JsonConvert.SerializeObject(habits, Formatting.Indented);
            await File.WriteAllTextAsync(_habitsFile, json);
        }

        public async Task<List<Pattern>> GetAllPatternsAsync()
        {
            if (!File.Exists(_patternsFile))
                return new List<Pattern>();

            var json = await File.ReadAllTextAsync(_patternsFile);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Pattern>();

            return JsonConvert.DeserializeObject<List<Pattern>>(json) ?? new List<Pattern>();
        }

        public async Task SavePatternAsync(Pattern pattern)
        {
            var patterns = await GetAllPatternsAsync();
            var existing = patterns.FirstOrDefault(p => p.Id == pattern.Id);
            
            if (existing != null)
            {
                patterns.Remove(existing);
            }
            
            patterns.Add(pattern);

            var json = JsonConvert.SerializeObject(patterns, Formatting.Indented);
            await File.WriteAllTextAsync(_patternsFile, json);
        }

        public async Task<AppSettings> GetSettingsAsync()
        {
            if (!File.Exists(_settingsFile))
                return new AppSettings();

            var json = await File.ReadAllTextAsync(_settingsFile);
            if (string.IsNullOrWhiteSpace(json))
                return new AppSettings();

            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsFile, json);
        }

        public async Task<List<CheckIn>> GetCheckInsInRangeAsync(DateTime startDate, DateTime endDate)
        {
            var checkIns = await GetAllCheckInsAsync();
            return checkIns.Where(c => c.Date.Date >= startDate.Date && c.Date.Date <= endDate.Date).ToList();
        }
    }
}
