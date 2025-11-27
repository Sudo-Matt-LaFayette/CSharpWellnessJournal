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
    public class SettingsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly IThemeService _themeService;
        private readonly IGoogleDriveService _googleDriveService;

        private bool _requirePassword;
        public bool RequirePassword
        {
            get => _requirePassword;
            set => SetProperty(ref _requirePassword, value);
        }

        private TimeSpan _morningReminderTime = new TimeSpan(8, 0, 0);
        public TimeSpan MorningReminderTime
        {
            get => _morningReminderTime;
            set => SetProperty(ref _morningReminderTime, value);
        }

        private TimeSpan _eveningReminderTime = new TimeSpan(20, 0, 0);
        public TimeSpan EveningReminderTime
        {
            get => _eveningReminderTime;
            set => SetProperty(ref _eveningReminderTime, value);
        }

        private bool _enableSystemNotifications = true;
        public bool EnableSystemNotifications
        {
            get => _enableSystemNotifications;
            set => SetProperty(ref _enableSystemNotifications, value);
        }

        private bool _enableInAppNotifications = true;
        public bool EnableInAppNotifications
        {
            get => _enableInAppNotifications;
            set => SetProperty(ref _enableInAppNotifications, value);
        }

        public ObservableCollection<string> AvailableThemes { get; } = new();
        private string _selectedTheme = "Default";
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    _themeService.ApplyTheme(value);
                }
            }
        }

        private bool _autoSyncToGoogleDrive;
        public bool AutoSyncToGoogleDrive
        {
            get => _autoSyncToGoogleDrive;
            set => SetProperty(ref _autoSyncToGoogleDrive, value);
        }

        public ObservableCollection<string> MistakeCategories { get; } = new();
        public ObservableCollection<Habit> Habits { get; } = new();

        private string? _newHabitName;
        public string? NewHabitName
        {
            get => _newHabitName;
            set => SetProperty(ref _newHabitName, value);
        }

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

        public ICommand SaveSettingsCommand { get; }
        public ICommand AddHabitCommand { get; }
        public ICommand DeleteHabitCommand { get; }
        public ICommand AddMistakeCategoryCommand { get; }
        public ICommand DeleteMistakeCategoryCommand { get; }
        public ICommand AuthenticateGoogleDriveCommand { get; }
        public ICommand SyncToDriveCommand { get; }

        public SettingsViewModel(
            IDataService dataService,
            INotificationService notificationService,
            IThemeService themeService,
            IGoogleDriveService googleDriveService)
        {
            _dataService = dataService;
            _notificationService = notificationService;
            _themeService = themeService;
            _googleDriveService = googleDriveService;

            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
            AddHabitCommand = new RelayCommand(AddHabit);
            DeleteHabitCommand = new RelayCommand<Habit>(DeleteHabit);
            AddMistakeCategoryCommand = new RelayCommand(AddMistakeCategory);
            DeleteMistakeCategoryCommand = new RelayCommand<string>(DeleteMistakeCategory);
            AuthenticateGoogleDriveCommand = new AsyncRelayCommand(AuthenticateGoogleDriveAsync);
            SyncToDriveCommand = new AsyncRelayCommand(SyncToDriveAsync);

            LoadSettings();
        }

        private async void LoadSettings()
        {
            var settings = await _dataService.GetSettingsAsync();
            
            RequirePassword = settings.RequirePassword;
            MorningReminderTime = settings.MorningReminderTime;
            EveningReminderTime = settings.EveningReminderTime;
            EnableSystemNotifications = settings.EnableSystemNotifications;
            EnableInAppNotifications = settings.EnableInAppNotifications;
            SelectedTheme = settings.CurrentTheme;
            AutoSyncToGoogleDrive = settings.AutoSyncToGoogleDrive;

            AvailableThemes.Clear();
            foreach (var theme in _themeService.GetAvailableThemes())
            {
                AvailableThemes.Add(theme);
            }

            MistakeCategories.Clear();
            foreach (var category in settings.MistakeCategories)
            {
                MistakeCategories.Add(category);
            }

            var habits = await _dataService.GetAllHabitsAsync();
            Habits.Clear();
            foreach (var habit in habits)
            {
                Habits.Add(habit);
            }
        }

        private async Task SaveSettingsAsync()
        {
            var settings = await _dataService.GetSettingsAsync();
            
            settings.RequirePassword = RequirePassword;
            settings.MorningReminderTime = MorningReminderTime;
            settings.EveningReminderTime = EveningReminderTime;
            settings.EnableSystemNotifications = EnableSystemNotifications;
            settings.EnableInAppNotifications = EnableInAppNotifications;
            settings.CurrentTheme = SelectedTheme;
            settings.AutoSyncToGoogleDrive = AutoSyncToGoogleDrive;
            settings.MistakeCategories = MistakeCategories.ToList();

            await _dataService.SaveSettingsAsync(settings);

            // Update reminders
            if (EnableSystemNotifications)
            {
                await _notificationService.ScheduleReminderAsync(MorningReminderTime, "Morning Check-In", "Time for your morning check-in!", true);
                await _notificationService.ScheduleReminderAsync(EveningReminderTime, "Evening Check-In", "Time for your evening check-in!", false);
            }
            else
            {
                _notificationService.CancelReminder(true);
                _notificationService.CancelReminder(false);
            }
            
            // Show encouraging success message
            SuccessMessage = "✅ Settings saved successfully! Your preferences have been updated. Thank you for customizing your wellness journey! 💚";
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

        private async void AddHabit()
        {
            if (string.IsNullOrWhiteSpace(NewHabitName))
                return;

            var habit = new Habit
            {
                Name = NewHabitName,
                IsCustom = true,
                Category = HabitCategory.Other
            };

            await _dataService.SaveHabitAsync(habit);
            Habits.Add(habit);
            NewHabitName = string.Empty;
        }

        private async void DeleteHabit(Habit? habit)
        {
            if (habit == null) return;

            await _dataService.DeleteHabitAsync(habit.Id);
            Habits.Remove(habit);
        }

        private void AddMistakeCategory()
        {
            MistakeCategories.Add("New Category");
        }

        private void DeleteMistakeCategory(string? category)
        {
            if (category == null) return;
            MistakeCategories.Remove(category);
        }

        private async Task AuthenticateGoogleDriveAsync()
        {
            await _googleDriveService.AuthenticateAsync();
        }

        private async Task SyncToDriveAsync()
        {
            await _googleDriveService.SyncToDriveAsync();
        }
    }
}

