using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyCheckInJournal.Services;

namespace DailyCheckInJournal.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly IPatternDetectionService _patternDetectionService;
        private readonly ILoggerService? _logger;

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToMorningCheckInCommand { get; }
        public ICommand NavigateToEveningCheckInCommand { get; }
        public ICommand NavigateToWeeklyReviewCommand { get; }
        public ICommand NavigateToVisualizationCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToCalendarCommand { get; }

        private readonly MorningCheckInViewModel _morningCheckInViewModel;
        private readonly EveningCheckInViewModel _eveningCheckInViewModel;
        private readonly WeeklyReviewViewModel _weeklyReviewViewModel;
        private readonly VisualizationViewModel _visualizationViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly CalendarViewModel _calendarViewModel;
        private readonly DayViewViewModel _dayViewViewModel;

        public MainViewModel(
            IDataService dataService,
            INotificationService notificationService,
            IPatternDetectionService patternDetectionService,
            MorningCheckInViewModel morningCheckInViewModel,
            EveningCheckInViewModel eveningCheckInViewModel,
            WeeklyReviewViewModel weeklyReviewViewModel,
            VisualizationViewModel visualizationViewModel,
            SettingsViewModel settingsViewModel,
            CalendarViewModel calendarViewModel,
            DayViewViewModel dayViewViewModel,
            ILoggerService? logger = null)
        {
            try
            {
                _logger = logger;
                _logger?.LogDebug("Initializing MainViewModel...");

                _dataService = dataService;
                _notificationService = notificationService;
                _patternDetectionService = patternDetectionService;
                _morningCheckInViewModel = morningCheckInViewModel;
                _eveningCheckInViewModel = eveningCheckInViewModel;
                _weeklyReviewViewModel = weeklyReviewViewModel;
                _visualizationViewModel = visualizationViewModel;
                _settingsViewModel = settingsViewModel;
                _calendarViewModel = calendarViewModel;
                _dayViewViewModel = dayViewViewModel;

                _logger?.LogDebug("Creating navigation commands...");
                NavigateToMorningCheckInCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Morning Check-In");
                    CurrentView = _morningCheckInViewModel;
                });
                NavigateToEveningCheckInCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Evening Check-In");
                    CurrentView = _eveningCheckInViewModel;
                });
                NavigateToWeeklyReviewCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Weekly Review");
                    CurrentView = _weeklyReviewViewModel;
                });
                NavigateToVisualizationCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Visualizations");
                    CurrentView = _visualizationViewModel;
                });
                NavigateToSettingsCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Settings");
                    CurrentView = _settingsViewModel;
                });
                NavigateToCalendarCommand = new RelayCommand(() => 
                {
                    _logger?.LogDebug("Navigating to Calendar");
                    CurrentView = _calendarViewModel;
                });

                // Subscribe to calendar's view day request
                _calendarViewModel.ViewDayRequested += (checkIn) =>
                {
                    _logger?.LogDebug($"Navigating to day view for {checkIn.Date:yyyy-MM-dd}");
                    _dayViewViewModel.CheckIn = checkIn;
                    CurrentView = _dayViewViewModel;
                };

                // Default to morning check-in
                _logger?.LogDebug("Setting default view to Morning Check-In");
                CurrentView = _morningCheckInViewModel;

                _logger?.LogInformation("MainViewModel initialized successfully");
                InitializeReminders();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing MainViewModel");
                throw;
            }
        }

        private async void InitializeReminders()
        {
            try
            {
                _logger?.LogDebug("Initializing reminders...");
                var settings = await _dataService.GetSettingsAsync();
                
                if (settings.EnableSystemNotifications)
                {
                    _logger?.LogDebug($"Scheduling morning reminder for {settings.MorningReminderTime}");
                    await _notificationService.ScheduleReminderAsync(
                        settings.MorningReminderTime,
                        "Morning Check-In",
                        "Time for your morning check-in!",
                        true);

                    _logger?.LogDebug($"Scheduling evening reminder for {settings.EveningReminderTime}");
                    await _notificationService.ScheduleReminderAsync(
                        settings.EveningReminderTime,
                        "Evening Check-In",
                        "Time for your evening check-in!",
                        false);
                    _logger?.LogInformation("Reminders scheduled successfully");
                }
                else
                {
                    _logger?.LogDebug("System notifications are disabled, skipping reminder scheduling");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing reminders");
            }
        }
    }
}

