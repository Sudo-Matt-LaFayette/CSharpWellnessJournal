using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyCheckInJournal.Models;

namespace DailyCheckInJournal.Services
{
    public class PatternDetectionService : IPatternDetectionService
    {
        private readonly IDataService _dataService;

        public PatternDetectionService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<List<Pattern>> DetectPatternsAsync(List<CheckIn> checkIns)
        {
            var patterns = new List<Pattern>();
            var existingPatterns = await _dataService.GetAllPatternsAsync();

            if (checkIns.Count < 7) // Need at least a week of data
                return patterns;

            // Detect energy patterns
            var energyPattern = DetectEnergyPattern(checkIns, existingPatterns);
            if (energyPattern != null)
                patterns.Add(energyPattern);

            // Detect overcommitment patterns
            var overcommitPattern = DetectOvercommitmentPattern(checkIns, existingPatterns);
            if (overcommitPattern != null)
                patterns.Add(overcommitPattern);

            // Detect mistake patterns
            var mistakePatterns = DetectMistakePatterns(checkIns, existingPatterns);
            patterns.AddRange(mistakePatterns);

            // Detect mood patterns
            var moodPattern = DetectMoodPattern(checkIns, existingPatterns);
            if (moodPattern != null)
                patterns.Add(moodPattern);

            // Detect sleep patterns
            var sleepPattern = DetectSleepPattern(checkIns, existingPatterns);
            if (sleepPattern != null)
                patterns.Add(sleepPattern);

            return patterns;
        }

        private Pattern? DetectEnergyPattern(List<CheckIn> checkIns, List<Pattern> existingPatterns)
        {
            var recentCheckIns = checkIns.OrderByDescending(c => c.Date).Take(14).ToList();
            if (recentCheckIns.Count < 7) return null;

            var morningEnergies = recentCheckIns
                .Where(c => c.Morning != null)
                .Select(c => c.Morning!.EnergyLevel)
                .ToList();

            if (morningEnergies.Count < 7) return null;

            var avgEnergy = morningEnergies.Average();
            var dayOfWeekEnergies = recentCheckIns
                .Where(c => c.Morning != null)
                .GroupBy(c => c.Date.DayOfWeek)
                .ToDictionary(g => g.Key.ToString(), g => g.Average(c => c.Morning!.EnergyLevel));

            // Check if there's a significant day-of-week pattern
            if (dayOfWeekEnergies.Count >= 3)
            {
                var maxDay = dayOfWeekEnergies.OrderByDescending(kvp => kvp.Value).First();
                var minDay = dayOfWeekEnergies.OrderBy(kvp => kvp.Value).First();

                if (maxDay.Value - minDay.Value >= 2) // Significant difference
                {
                    var existing = existingPatterns.FirstOrDefault(p => 
                        p.Type == PatternType.EnergyPattern && p.IsActive);
                    
                    var pattern = existing ?? new Pattern
                    {
                        Type = PatternType.EnergyPattern,
                        IsActive = true
                    };

                    pattern.Title = "Energy Pattern Detected";
                    pattern.Description = $"Your energy is typically highest on {maxDay.Key} and lowest on {minDay.Key}.";
                    pattern.Data = new Dictionary<string, object>
                    {
                        { "HighestDay", maxDay.Key },
                        { "LowestDay", minDay.Key },
                        { "AverageEnergy", avgEnergy }
                    };

                    return pattern;
                }
            }

            return null;
        }

        private Pattern? DetectOvercommitmentPattern(List<CheckIn> checkIns, List<Pattern> existingPatterns)
        {
            var recentCheckIns = checkIns.OrderByDescending(c => c.Date).Take(14).ToList();
            var overcommittedDays = recentCheckIns
                .Where(c => c.Evening?.Overcommitted == true)
                .ToList();

            if (overcommittedDays.Count >= 3)
            {
                var dayOfWeek = overcommittedDays
                    .GroupBy(c => c.Date.DayOfWeek)
                    .OrderByDescending(g => g.Count())
                    .First();

                if (dayOfWeek.Count() >= 2)
                {
                    var existing = existingPatterns.FirstOrDefault(p => 
                        p.Type == PatternType.OvercommitmentPattern && p.IsActive);
                    
                    var pattern = existing ?? new Pattern
                    {
                        Type = PatternType.OvercommitmentPattern,
                        IsActive = true
                    };

                    pattern.Title = "Overcommitment Pattern";
                    pattern.Description = $"You tend to overcommit on {dayOfWeek.Key}s. Consider scheduling lighter days.";
                    pattern.Data = new Dictionary<string, object>
                    {
                        { "DayOfWeek", dayOfWeek.Key.ToString() },
                        { "Frequency", dayOfWeek.Count() }
                    };

                    return pattern;
                }
            }

            return null;
        }

        private List<Pattern> DetectMistakePatterns(List<CheckIn> checkIns, List<Pattern> existingPatterns)
        {
            var patterns = new List<Pattern>();
            var recentCheckIns = checkIns.OrderByDescending(c => c.Date).Take(30).ToList();

            var mistakeFrequency = recentCheckIns
                .Where(c => c.Evening != null && c.Evening.CommonMistakes.Any())
                .SelectMany(c => c.Evening!.CommonMistakes)
                .GroupBy(m => m.Category)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var mistake in mistakeFrequency.Where(kvp => kvp.Value >= 3))
            {
                var existing = existingPatterns.FirstOrDefault(p => 
                    p.Type == PatternType.MistakePattern && 
                    p.Data.ContainsKey("Category") &&
                    p.Data["Category"].ToString() == mistake.Key &&
                    p.IsActive);
                
                var pattern = existing ?? new Pattern
                {
                    Type = PatternType.MistakePattern,
                    IsActive = true
                };

                pattern.Title = $"Frequent Mistake: {mistake.Key}";
                pattern.Description = $"This mistake has occurred {mistake.Value} times in the last 30 days. Consider strategies to address this.";
                pattern.Data = new Dictionary<string, object>
                {
                    { "Category", mistake.Key },
                    { "Frequency", mistake.Value }
                };

                patterns.Add(pattern);
            }

            return patterns;
        }

        private Pattern? DetectMoodPattern(List<CheckIn> checkIns, List<Pattern> existingPatterns)
        {
            var recentCheckIns = checkIns.OrderByDescending(c => c.Date).Take(14).ToList();
            var moods = recentCheckIns
                .Where(c => c.Morning?.EmotionalState != null)
                .Select(c => c.Morning!.EmotionalState!.OverallMood)
                .ToList();

            if (moods.Count < 7) return null;

            var avgMood = moods.Average();
            if (avgMood <= 4) // Low mood pattern
            {
                var existing = existingPatterns.FirstOrDefault(p => 
                    p.Type == PatternType.MoodPattern && p.IsActive);
                
                var pattern = existing ?? new Pattern
                {
                    Type = PatternType.MoodPattern,
                    IsActive = true
                };

                pattern.Title = "Low Mood Pattern";
                pattern.Description = $"Your average mood has been low ({avgMood:F1}/10) over the past 2 weeks. Consider reviewing coping strategies.";
                pattern.Data = new Dictionary<string, object>
                {
                    { "AverageMood", avgMood }
                };

                return pattern;
            }

            return null;
        }

        private Pattern? DetectSleepPattern(List<CheckIn> checkIns, List<Pattern> existingPatterns)
        {
            var recentCheckIns = checkIns.OrderByDescending(c => c.Date).Take(14).ToList();
            var sleepQualities = recentCheckIns
                .Where(c => c.Morning?.SleepQuality != null)
                .Select(c => c.Morning!.SleepQuality!.Quality)
                .ToList();

            if (sleepQualities.Count < 7) return null;

            var avgSleepQuality = sleepQualities.Average();
            if (avgSleepQuality <= 5) // Poor sleep pattern
            {
                var existing = existingPatterns.FirstOrDefault(p => 
                    p.Type == PatternType.SleepPattern && p.IsActive);
                
                var pattern = existing ?? new Pattern
                {
                    Type = PatternType.SleepPattern,
                    IsActive = true
                };

                pattern.Title = "Sleep Quality Pattern";
                pattern.Description = $"Your average sleep quality has been low ({avgSleepQuality:F1}/10). Poor sleep may be affecting your daily functioning.";
                pattern.Data = new Dictionary<string, object>
                {
                    { "AverageSleepQuality", avgSleepQuality }
                };

                return pattern;
            }

            return null;
        }

        public async Task<List<Pattern>> GetActivePatternsAsync()
        {
            var patterns = await _dataService.GetAllPatternsAsync();
            return patterns.Where(p => p.IsActive).ToList();
        }
    }
}

