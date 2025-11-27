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
    public class MorningCheckInViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        private int _energyLevel = 5;
        public int EnergyLevel
        {
            get => _energyLevel;
            set => SetProperty(ref _energyLevel, value);
        }

        private string? _mustDoToday;
        public string? MustDoToday
        {
            get => _mustDoToday;
            set => SetProperty(ref _mustDoToday, value);
        }

        private int _capacityFeeling = 5;
        public int CapacityFeeling
        {
            get => _capacityFeeling;
            set => SetProperty(ref _capacityFeeling, value);
        }

        private SleepQuality? _sleepQuality;
        public SleepQuality? SleepQuality
        {
            get => _sleepQuality;
            set => SetProperty(ref _sleepQuality, value);
        }

        public ObservableCollection<MedicationEntry> Medications { get; } = new();
        public ObservableCollection<HabitEntry> HabitEntries { get; } = new();
        public ObservableCollection<string> GratitudeItems { get; } = new();
        public ObservableCollection<TriggerEntry> Triggers { get; } = new();
        public ObservableCollection<CopingStrategy> CopingStrategies { get; } = new();

        private SensoryState? _sensoryState;
        public SensoryState? SensoryState
        {
            get => _sensoryState;
            set => SetProperty(ref _sensoryState, value);
        }

        private ExecutiveFunctionState? _executiveFunction;
        public ExecutiveFunctionState? ExecutiveFunction
        {
            get => _executiveFunction;
            set => SetProperty(ref _executiveFunction, value);
        }

        private EmotionalState? _emotionalState;
        public EmotionalState? EmotionalState
        {
            get => _emotionalState;
            set => SetProperty(ref _emotionalState, value);
        }

        public ObservableCollection<Habit> AvailableHabits { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand AddMedicationCommand { get; }
        public ICommand AddGratitudeCommand { get; }
        public ICommand AddTriggerCommand { get; }
        public ICommand AddCopingStrategyCommand { get; }

        public MorningCheckInViewModel(IDataService dataService)
        {
            _dataService = dataService;
            SaveCommand = new AsyncRelayCommand(SaveCheckInAsync);
            AddMedicationCommand = new RelayCommand(() => Medications.Add(new MedicationEntry { TimeTaken = DateTime.Now }));
            AddGratitudeCommand = new RelayCommand(() => GratitudeItems.Add(string.Empty));
            AddTriggerCommand = new RelayCommand(() => Triggers.Add(new TriggerEntry()));
            AddCopingStrategyCommand = new RelayCommand(() => CopingStrategies.Add(new CopingStrategy()));

            LoadTodayCheckIn();
            LoadHabits();
        }

        private async void LoadTodayCheckIn()
        {
            var today = DateTime.Today;
            var checkIn = await _dataService.GetCheckInAsync(today);
            
            if (checkIn?.Morning != null)
            {
                var morning = checkIn.Morning;
                EnergyLevel = morning.EnergyLevel;
                MustDoToday = morning.MustDoToday;
                CapacityFeeling = morning.CapacityFeeling;
                SleepQuality = morning.SleepQuality;
                
                Medications.Clear();
                foreach (var med in morning.Medications)
                    Medications.Add(med);

                HabitEntries.Clear();
                foreach (var habit in morning.HabitEntries)
                    HabitEntries.Add(habit);

                GratitudeItems.Clear();
                foreach (var item in morning.GratitudeItems)
                    GratitudeItems.Add(item);

                Triggers.Clear();
                foreach (var trigger in morning.Triggers)
                    Triggers.Add(trigger);

                CopingStrategies.Clear();
                foreach (var strategy in morning.CopingStrategies)
                    CopingStrategies.Add(strategy);

                SensoryState = morning.SensoryState;
                ExecutiveFunction = morning.ExecutiveFunction;
                EmotionalState = morning.EmotionalState;
            }
            else
            {
                SleepQuality = new SleepQuality();
                SensoryState = new SensoryState();
                ExecutiveFunction = new ExecutiveFunctionState();
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

        private async Task SaveCheckInAsync()
        {
            var today = DateTime.Today;
            var checkIn = await _dataService.GetCheckInAsync(today) ?? new CheckIn { Date = today };

            checkIn.Morning = new MorningCheckIn
            {
                EnergyLevel = EnergyLevel,
                MustDoToday = MustDoToday,
                CapacityFeeling = CapacityFeeling,
                SleepQuality = SleepQuality,
                Medications = Medications.ToList(),
                HabitEntries = HabitEntries.ToList(),
                GratitudeItems = GratitudeItems.Where(g => !string.IsNullOrWhiteSpace(g)).ToList(),
                Triggers = Triggers.ToList(),
                CopingStrategies = CopingStrategies.ToList(),
                SensoryState = SensoryState,
                ExecutiveFunction = ExecutiveFunction,
                EmotionalState = EmotionalState,
                CheckInTime = DateTime.Now
            };

            await _dataService.SaveCheckInAsync(checkIn);
        }
    }
}
