using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyCheckInJournal.Models;
using DailyCheckInJournal.Services;

namespace DailyCheckInJournal.ViewModels
{
    public class WeeklyReviewViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly IPatternDetectionService _patternDetectionService;

        private DateTime _reviewStartDate = DateTime.Today.AddDays(-6);
        public DateTime ReviewStartDate
        {
            get => _reviewStartDate;
            set
            {
                if (SetProperty(ref _reviewStartDate, value))
                {
                    LoadWeeklyData();
                }
            }
        }

        public ObservableCollection<CheckIn> WeeklyCheckIns { get; } = new();
        public ObservableCollection<Pattern> DetectedPatterns { get; } = new();

        private string? _weeklySummary;
        public string? WeeklySummary
        {
            get => _weeklySummary;
            set => SetProperty(ref _weeklySummary, value);
        }

        private double _averageEnergy;
        public double AverageEnergy
        {
            get => _averageEnergy;
            set => SetProperty(ref _averageEnergy, value);
        }

        private double _averageMood;
        public double AverageMood
        {
            get => _averageMood;
            set => SetProperty(ref _averageMood, value);
        }

        private int _mustDoCompletionRate;
        public int MustDoCompletionRate
        {
            get => _mustDoCompletionRate;
            set => SetProperty(ref _mustDoCompletionRate, value);
        }

        private int _overcommitmentCount;
        public int OvercommitmentCount
        {
            get => _overcommitmentCount;
            set => SetProperty(ref _overcommitmentCount, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand PreviousWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

        public WeeklyReviewViewModel(IDataService dataService, IPatternDetectionService patternDetectionService)
        {
            _dataService = dataService;
            _patternDetectionService = patternDetectionService;
            
            GenerateReportCommand = new AsyncRelayCommand(GenerateReportAsync);
            PreviousWeekCommand = new RelayCommand(() => ReviewStartDate = ReviewStartDate.AddDays(-7));
            NextWeekCommand = new RelayCommand(() => ReviewStartDate = ReviewStartDate.AddDays(7));

            LoadWeeklyData();
        }

        private async void LoadWeeklyData()
        {
            var endDate = ReviewStartDate.AddDays(6);
            var checkIns = await _dataService.GetCheckInsInRangeAsync(ReviewStartDate, endDate);
            
            WeeklyCheckIns.Clear();
            foreach (var checkIn in checkIns.OrderBy(c => c.Date))
            {
                WeeklyCheckIns.Add(checkIn);
            }

            // Detect patterns
            var allCheckIns = await _dataService.GetAllCheckInsAsync();
            var patterns = await _patternDetectionService.DetectPatternsAsync(allCheckIns);
            
            DetectedPatterns.Clear();
            foreach (var pattern in patterns.Where(p => p.IsActive))
            {
                DetectedPatterns.Add(pattern);
            }

            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            var morningEnergies = WeeklyCheckIns
                .Where(c => c.Morning != null)
                .Select(c => c.Morning!.EnergyLevel)
                .ToList();

            AverageEnergy = morningEnergies.Any() ? morningEnergies.Average() : 0;

            var moods = WeeklyCheckIns
                .Where(c => c.Morning?.EmotionalState != null)
                .Select(c => c.Morning!.EmotionalState!.OverallMood)
                .ToList();

            AverageMood = moods.Any() ? moods.Average() : 0;

            var completed = WeeklyCheckIns.Count(c => c.Evening?.MustDoCompleted == true);
            var total = WeeklyCheckIns.Count(c => c.Evening != null);
            MustDoCompletionRate = total > 0 ? (int)((double)completed / total * 100) : 0;

            OvercommitmentCount = WeeklyCheckIns.Count(c => c.Evening?.Overcommitted == true);
        }

        private async Task GenerateReportAsync()
        {
            var report = $"Weekly Review Report\n";
            report += $"Period: {ReviewStartDate:MMM dd} - {ReviewStartDate.AddDays(6):MMM dd, yyyy}\n\n";
            
            report += $"Statistics:\n";
            report += $"  Average Energy Level: {AverageEnergy:F1}/10\n";
            report += $"  Average Mood: {AverageMood:F1}/10\n";
            report += $"  Must-Do Completion Rate: {MustDoCompletionRate}%\n";
            report += $"  Days Overcommitted: {OvercommitmentCount}\n\n";

            report += $"Detected Patterns:\n";
            foreach (var pattern in DetectedPatterns)
            {
                report += $"  - {pattern.Title}: {pattern.Description}\n";
            }

            report += $"\nCommon Mistakes:\n";
            var mistakes = WeeklyCheckIns
                .Where(c => c.Evening != null)
                .SelectMany(c => c.Evening!.CommonMistakes)
                .GroupBy(m => m.Category)
                .OrderByDescending(g => g.Count())
                .Take(5);

            foreach (var mistake in mistakes)
            {
                report += $"  - {mistake.Key}: {mistake.Count()} times\n";
            }

            WeeklySummary = report;
        }
    }
}

