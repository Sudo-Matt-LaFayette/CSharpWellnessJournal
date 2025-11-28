using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DailyCheckInJournal.Models;
using DailyCheckInJournal.Services;

namespace DailyCheckInJournal.ViewModels
{
    public class DayViewViewModel : ObservableObject
    {
        private readonly ILoggerService? _logger;
        private CheckIn? _checkIn;
        private DateTime _viewDate;

        public DateTime ViewDate
        {
            get => _viewDate;
            set => SetProperty(ref _viewDate, value);
        }

        public CheckIn? CheckIn
        {
            get => _checkIn;
            set
            {
                if (SetProperty(ref _checkIn, value))
                {
                    LoadCheckInData();
                }
            }
        }

        // Morning Check-In Properties
        public bool HasMorningCheckIn => CheckIn?.Morning != null;
        public int MorningEnergyLevel => CheckIn?.Morning?.EnergyLevel ?? 0;
        public string? MorningMustDoToday => CheckIn?.Morning?.MustDoToday;
        public int MorningCapacityFeeling => CheckIn?.Morning?.CapacityFeeling ?? 0;
        public SleepQuality? MorningSleepQuality => CheckIn?.Morning?.SleepQuality;
        public ObservableCollection<MedicationEntry> MorningMedications { get; } = new();
        public ObservableCollection<HabitEntry> MorningHabitEntries { get; } = new();
        public ObservableCollection<string> MorningGratitudeItems { get; } = new();
        public ObservableCollection<TriggerEntry> MorningTriggers { get; } = new();
        public ObservableCollection<CopingStrategy> MorningCopingStrategies { get; } = new();
        public SensoryState? MorningSensoryState => CheckIn?.Morning?.SensoryState;
        public ExecutiveFunctionState? MorningExecutiveFunction => CheckIn?.Morning?.ExecutiveFunction;
        public EmotionalState? MorningEmotionalState => CheckIn?.Morning?.EmotionalState;
        public DateTime? MorningCheckInTime => CheckIn?.Morning?.CheckInTime;

        // Evening Check-In Properties
        public bool HasEveningCheckIn => CheckIn?.Evening != null;
        public bool? EveningMustDoCompleted => CheckIn?.Evening?.MustDoCompleted;
        public int EveningEnergyLevel => CheckIn?.Evening?.EnergyLevel ?? 0;
        public bool? EveningOvercommitted => CheckIn?.Evening?.Overcommitted;
        public ObservableCollection<MistakeEntry> EveningCommonMistakes { get; } = new();
        public EmotionalState? EveningEmotionalState => CheckIn?.Evening?.EmotionalState;
        public ObservableCollection<string> EveningGratitudeItems { get; } = new();
        public ObservableCollection<TriggerEntry> EveningTriggers { get; } = new();
        public ObservableCollection<CopingStrategy> EveningCopingStrategies { get; } = new();
        public ObservableCollection<HabitEntry> EveningHabitEntries { get; } = new();
        public string? EveningNotes => CheckIn?.Evening?.Notes;
        public DateTime? EveningCheckInTime => CheckIn?.Evening?.CheckInTime;

        public DayViewViewModel(ILoggerService? logger = null)
        {
            _logger = logger;
            ViewDate = DateTime.Today;
        }

        private void LoadCheckInData()
        {
            try
            {
                _logger?.LogDebug("Loading check-in data into DayViewViewModel...");

                // Clear existing data
                MorningMedications.Clear();
                MorningHabitEntries.Clear();
                MorningGratitudeItems.Clear();
                MorningTriggers.Clear();
                MorningCopingStrategies.Clear();
                EveningCommonMistakes.Clear();
                EveningGratitudeItems.Clear();
                EveningTriggers.Clear();
                EveningCopingStrategies.Clear();
                EveningHabitEntries.Clear();

                if (CheckIn == null)
                {
                    _logger?.LogDebug("No check-in data to load");
                    OnPropertyChanged(nameof(HasMorningCheckIn));
                    OnPropertyChanged(nameof(HasEveningCheckIn));
                    return;
                }

                ViewDate = CheckIn.Date;

                // Load morning data
                if (CheckIn.Morning != null)
                {
                    foreach (var med in CheckIn.Morning.Medications)
                        MorningMedications.Add(med);
                    foreach (var habit in CheckIn.Morning.HabitEntries)
                        MorningHabitEntries.Add(habit);
                    foreach (var gratitude in CheckIn.Morning.GratitudeItems)
                        MorningGratitudeItems.Add(gratitude);
                    foreach (var trigger in CheckIn.Morning.Triggers)
                        MorningTriggers.Add(trigger);
                    foreach (var strategy in CheckIn.Morning.CopingStrategies)
                        MorningCopingStrategies.Add(strategy);
                }

                // Load evening data
                if (CheckIn.Evening != null)
                {
                    foreach (var mistake in CheckIn.Evening.CommonMistakes)
                        EveningCommonMistakes.Add(mistake);
                    foreach (var gratitude in CheckIn.Evening.GratitudeItems)
                        EveningGratitudeItems.Add(gratitude);
                    foreach (var trigger in CheckIn.Evening.Triggers)
                        EveningTriggers.Add(trigger);
                    foreach (var strategy in CheckIn.Evening.CopingStrategies)
                        EveningCopingStrategies.Add(strategy);
                    foreach (var habit in CheckIn.Evening.HabitEntries)
                        EveningHabitEntries.Add(habit);
                }

                // Notify all properties changed
                OnPropertyChanged(nameof(HasMorningCheckIn));
                OnPropertyChanged(nameof(MorningEnergyLevel));
                OnPropertyChanged(nameof(MorningMustDoToday));
                OnPropertyChanged(nameof(MorningCapacityFeeling));
                OnPropertyChanged(nameof(MorningSleepQuality));
                OnPropertyChanged(nameof(MorningSensoryState));
                OnPropertyChanged(nameof(MorningExecutiveFunction));
                OnPropertyChanged(nameof(MorningEmotionalState));
                OnPropertyChanged(nameof(MorningCheckInTime));

                OnPropertyChanged(nameof(HasEveningCheckIn));
                OnPropertyChanged(nameof(EveningMustDoCompleted));
                OnPropertyChanged(nameof(EveningEnergyLevel));
                OnPropertyChanged(nameof(EveningOvercommitted));
                OnPropertyChanged(nameof(EveningEmotionalState));
                OnPropertyChanged(nameof(EveningNotes));
                OnPropertyChanged(nameof(EveningCheckInTime));

                _logger?.LogInformation("Check-in data loaded successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading check-in data");
            }
        }
    }
}

