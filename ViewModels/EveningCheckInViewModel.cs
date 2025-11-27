using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyCheckInJournal.Models;
using DailyCheckInJournal.Services;

namespace DailyCheckInJournal.ViewModels
{
    public class EveningCheckInViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        private bool? _mustDoCompleted;
        public bool? MustDoCompleted
        {
            get => _mustDoCompleted;
            set => SetProperty(ref _mustDoCompleted, value);
        }

        private int _energyLevel = 5;
        public int EnergyLevel
        {
            get => _energyLevel;
            set => SetProperty(ref _energyLevel, value);
        }

        private bool? _overcommitted;
        public bool? Overcommitted
        {
            get => _overcommitted;
            set => SetProperty(ref _overcommitted, value);
        }

        public ObservableCollection<MistakeEntry> CommonMistakes { get; } = new();
        public ObservableCollection<HabitEntry> HabitEntries { get; } = new();
        public ObservableCollection<string> GratitudeItems { get; } = new();
        public ObservableCollection<TriggerEntry> Triggers { get; } = new();
        public ObservableCollection<CopingStrategy> CopingStrategies { get; } = new();

        private EmotionalState? _emotionalState;
        public EmotionalState? EmotionalState
        {
            get => _emotionalState;
            set => SetProperty(ref _emotionalState, value);
        }

        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public ObservableCollection<Habit> AvailableHabits { get; } = new();
        public ObservableCollection<string> MistakeCategories { get; } = new();

        private string? _successMessage;
        public string? SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        private bool _showSuccessMessage;
        public bool ShowSuccessMessage
        {
            get => _showSuccessMessage;
            set => SetProperty(ref _showSuccessMessage, value);
        }

        private string? _mustDoFromMorning;
        public string? MustDoFromMorning
        {
            get => _mustDoFromMorning;
            set => SetProperty(ref _mustDoFromMorning, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand AddMistakeCommand { get; }
        public ICommand AddGratitudeCommand { get; }
        public ICommand AddTriggerCommand { get; }
        public ICommand AddCopingStrategyCommand { get; }

        public EveningCheckInViewModel(IDataService dataService)
        {
            _dataService = dataService;
            SaveCommand = new AsyncRelayCommand(SaveCheckInAsync);
            AddMistakeCommand = new RelayCommand(() => CommonMistakes.Add(new MistakeEntry()));
            AddGratitudeCommand = new RelayCommand(() => GratitudeItems.Add(string.Empty));
            AddTriggerCommand = new RelayCommand(() => Triggers.Add(new TriggerEntry()));
            AddCopingStrategyCommand = new RelayCommand(() => CopingStrategies.Add(new CopingStrategy()));

            LoadTodayCheckIn();
            LoadHabits();
            LoadMistakeCategories();
        }

        private async void LoadTodayCheckIn()
        {
            var today = DateTime.Today;
            var checkIn = await _dataService.GetCheckInAsync(today);
            
            if (checkIn?.Morning != null)
            {
                MustDoFromMorning = checkIn.Morning.MustDoToday;
            }

            if (checkIn?.Evening != null)
            {
                var evening = checkIn.Evening;
                MustDoCompleted = evening.MustDoCompleted;
                EnergyLevel = evening.EnergyLevel;
                Overcommitted = evening.Overcommitted;
                Notes = evening.Notes;
                
                CommonMistakes.Clear();
                foreach (var mistake in evening.CommonMistakes)
                    CommonMistakes.Add(mistake);

                HabitEntries.Clear();
                foreach (var habit in evening.HabitEntries)
                    HabitEntries.Add(habit);

                GratitudeItems.Clear();
                foreach (var item in evening.GratitudeItems)
                    GratitudeItems.Add(item);

                Triggers.Clear();
                foreach (var trigger in evening.Triggers)
                    Triggers.Add(trigger);

                CopingStrategies.Clear();
                foreach (var strategy in evening.CopingStrategies)
                    CopingStrategies.Add(strategy);

                EmotionalState = evening.EmotionalState;
            }
            else
            {
                EmotionalState = new EmotionalState();
            }
        }

        private async void LoadHabits()
        {
            var habits = await _dataService.GetAllHabitsAsync();
            AvailableHabits.Clear();
            foreach (var habit in habits.Where(h => h.IsActive))
            {
                AvailableHabits.Add(habit);
                if (!HabitEntries.Any(he => he.HabitName == habit.Name))
                {
                    HabitEntries.Add(new HabitEntry { HabitName = habit.Name });
                }
            }
        }

        private async void LoadMistakeCategories()
        {
            var settings = await _dataService.GetSettingsAsync();
            MistakeCategories.Clear();
            foreach (var category in settings.MistakeCategories)
            {
                MistakeCategories.Add(category);
            }
        }

        private async Task SaveCheckInAsync()
        {
            var today = DateTime.Today;
            var checkIn = await _dataService.GetCheckInAsync(today) ?? new CheckIn { Date = today };

            checkIn.Evening = new EveningCheckIn
            {
                MustDoCompleted = MustDoCompleted,
                EnergyLevel = EnergyLevel,
                Overcommitted = Overcommitted,
                CommonMistakes = CommonMistakes.ToList(),
                HabitEntries = HabitEntries.ToList(),
                GratitudeItems = GratitudeItems.Where(g => !string.IsNullOrWhiteSpace(g)).ToList(),
                Triggers = Triggers.ToList(),
                CopingStrategies = CopingStrategies.ToList(),
                EmotionalState = EmotionalState,
                Notes = Notes,
                CheckInTime = DateTime.Now
            };

            await _dataService.SaveCheckInAsync(checkIn);
            
            // Show encouraging success message
            SuccessMessage = "💙 Well done! Your evening check-in has been saved. Reflecting on your day is a powerful act of self-care. Keep going! 🌙";
            ShowSuccessMessage = true;
            
            // Clear the message after 5 seconds
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += (s, e) =>
            {
                ShowSuccessMessage = false;
                SuccessMessage = null;
                timer.Stop();
            };
            timer.Start();
        }
    }
}

