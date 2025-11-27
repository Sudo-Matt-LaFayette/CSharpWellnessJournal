using System;
using System.Windows;
using DailyCheckInJournal.Services;
using DailyCheckInJournal.ViewModels;

namespace DailyCheckInJournal
{
    public partial class MainWindow : Window
    {
        private readonly ILoggerService? _logger;

        public MainWindow(MainViewModel viewModel, ILoggerService? logger = null)
        {
            try
            {
                _logger = logger;
                _logger?.LogDebug("Initializing MainWindow...");
                InitializeComponent();
                DataContext = viewModel;
                _logger?.LogInformation("MainWindow initialized successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing MainWindow");
                throw;
            }
        }
    }
}

