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

namespace DailyCheckInJournal.ViewModels
{
    public class CalendarViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly ILoggerService? _logger;
        private DateTime? _selectedDate;
        private CheckIn? _selectedCheckIn;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value) && value.HasValue)
                {
                    _logger?.LogDebug($"Date selected: {value.Value:yyyy-MM-dd}");
                    LoadCheckInForDate(value.Value);
                }
            }
        }

        public CheckIn? SelectedCheckIn
        {
            get => _selectedCheckIn;
            set => SetProperty(ref _selectedCheckIn, value);
        }

        public ObservableCollection<DateTime> DatesWithCheckIns { get; } = new();

        public ICommand ViewDayCommand { get; }
        public ICommand LoadCheckInsCommand { get; }

        public event Action<CheckIn>? ViewDayRequested;

        public CalendarViewModel(IDataService dataService, ILoggerService? logger = null)
        {
            _dataService = dataService;
            _logger = logger;
            
            ViewDayCommand = new RelayCommand(() =>
            {
                if (SelectedDate.HasValue && SelectedCheckIn != null)
                {
                    _logger?.LogDebug($"Requesting full day view for {SelectedDate.Value:yyyy-MM-dd}");
                    ViewDayRequested?.Invoke(SelectedCheckIn);
                }
            }, () => SelectedDate.HasValue && SelectedCheckIn != null);

            LoadCheckInsCommand = new RelayCommand(async () => await LoadCheckInsAsync());

            // Load check-ins on initialization
            _ = LoadCheckInsAsync();
        }

        private async Task LoadCheckInsAsync()
        {
            try
            {
                _logger?.LogDebug("Loading check-ins for calendar...");
                var checkIns = await _dataService.GetAllCheckInsAsync();
                
                DatesWithCheckIns.Clear();
                foreach (var checkIn in checkIns)
                {
                    DatesWithCheckIns.Add(checkIn.Date.Date);
                }

                _logger?.LogInformation($"Loaded {DatesWithCheckIns.Count} dates with check-ins");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading check-ins for calendar");
            }
        }

        private async void LoadCheckInForDate(DateTime date)
        {
            try
            {
                _logger?.LogDebug($"Loading check-in for date: {date:yyyy-MM-dd}");
                var checkIn = await _dataService.GetCheckInAsync(date);
                SelectedCheckIn = checkIn;
                
                if (checkIn == null)
                {
                    _logger?.LogDebug($"No check-in found for {date:yyyy-MM-dd}");
                }
                else
                {
                    _logger?.LogDebug($"Check-in loaded for {date:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error loading check-in for date {date:yyyy-MM-dd}");
            }
        }

        public bool HasCheckIn(DateTime date)
        {
            return DatesWithCheckIns.Contains(date.Date);
        }
    }
}

