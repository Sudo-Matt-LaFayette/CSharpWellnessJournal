using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyCheckInJournal.Models;
using DailyCheckInJournal.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace DailyCheckInJournal.ViewModels
{
    public class VisualizationViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly IPatternDetectionService _patternDetectionService;

        public ObservableCollection<ISeries> EnergySeries { get; } = new();
        public ObservableCollection<ISeries> MoodSeries { get; } = new();
        public ObservableCollection<ISeries> SleepSeries { get; } = new();
        public ObservableCollection<ISeries> MistakeFrequencySeries { get; } = new();

        public ObservableCollection<Pattern> ActivePatterns { get; } = new();

        private DateTime _startDate = DateTime.Today.AddDays(-30);
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    LoadVisualizations();
                }
            }
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    LoadVisualizations();
                }
            }
        }

        public ICommand RefreshCommand { get; }

        public VisualizationViewModel(IDataService dataService, IPatternDetectionService patternDetectionService)
        {
            _dataService = dataService;
            _patternDetectionService = patternDetectionService;
            RefreshCommand = new AsyncRelayCommand(LoadVisualizationsAsync);

            LoadVisualizations();
        }

        private async void LoadVisualizations()
        {
            await LoadVisualizationsAsync();
        }

        private async Task LoadVisualizationsAsync()
        {
            var checkIns = await _dataService.GetCheckInsInRangeAsync(StartDate, EndDate);
            var patterns = await _patternDetectionService.GetActivePatternsAsync();

            ActivePatterns.Clear();
            foreach (var pattern in patterns)
            {
                ActivePatterns.Add(pattern);
            }

            // Energy Level Chart
            EnergySeries.Clear();
            var energyData = checkIns
                .Where(c => c.Morning != null)
                .OrderBy(c => c.Date)
                .Select(c => new { c.Date, Value = c.Morning!.EnergyLevel })
                .ToList();

            if (energyData.Any())
            {
                EnergySeries.Add(new LineSeries<int>
                {
                    Values = energyData.Select(d => d.Value).ToArray(),
                    Name = "Morning Energy",
                    GeometrySize = 8
                });
            }

            // Mood Chart
            MoodSeries.Clear();
            var moodData = checkIns
                .Where(c => c.Morning?.EmotionalState != null)
                .OrderBy(c => c.Date)
                .Select(c => new { c.Date, Value = c.Morning!.EmotionalState!.OverallMood })
                .ToList();

            if (moodData.Any())
            {
                MoodSeries.Add(new LineSeries<int>
                {
                    Values = moodData.Select(d => d.Value).ToArray(),
                    Name = "Mood",
                    GeometrySize = 8
                });
            }

            // Sleep Quality Chart
            SleepSeries.Clear();
            var sleepData = checkIns
                .Where(c => c.Morning?.SleepQuality != null)
                .OrderBy(c => c.Date)
                .Select(c => new { c.Date, Value = c.Morning!.SleepQuality!.Quality })
                .ToList();

            if (sleepData.Any())
            {
                SleepSeries.Add(new LineSeries<int>
                {
                    Values = sleepData.Select(d => d.Value).ToArray(),
                    Name = "Sleep Quality",
                    GeometrySize = 8
                });
            }

            // Mistake Frequency Chart
            MistakeFrequencySeries.Clear();
            var mistakeData = checkIns
                .Where(c => c.Evening != null)
                .SelectMany(c => c.Evening!.CommonMistakes)
                .GroupBy(m => m.Category)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            if (mistakeData.Any())
            {
                MistakeFrequencySeries.Add(new ColumnSeries<int>
                {
                    Values = mistakeData.Select(g => g.Count()).ToArray(),
                    Name = "Mistakes"
                });
            }
        }
    }
}

