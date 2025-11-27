using System;
using System.Windows;
using System.Windows.Threading;
using DailyCheckInJournal.Services;
using DailyCheckInJournal.ViewModels;
using DailyCheckInJournal.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DailyCheckInJournal
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private ILoggerService? _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize logger first
                _logger = new LoggerService();
                _logger.LogInformation("Application starting...");

                // Configure global exception handlers
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                DispatcherUnhandledException += OnDispatcherUnhandledException;

                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                _logger.LogInformation("Services configured successfully");

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                _logger.LogInformation("Main window created, showing...");
                mainWindow.Show();

                _logger.LogInformation("Application started successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Fatal error during application startup");
                MessageBox.Show(
                    $"A fatal error occurred during startup:\n\n{ex.Message}\n\nCheck the log file for details.",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            try
            {
                _logger?.LogDebug("Configuring services...");

                // Logger must be registered first
                services.AddSingleton<ILoggerService>(_logger!);

                // Services - inject logger where needed
                _logger?.LogDebug("Registering services...");
                services.AddSingleton<IDataService>(sp => 
                {
                    var logger = sp.GetRequiredService<ILoggerService>();
                    return new DataService(logger);
                });
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<IPatternDetectionService, PatternDetectionService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IGoogleDriveService, GoogleDriveService>();

                // ViewModels - inject logger where needed
                _logger?.LogDebug("Registering ViewModels...");
                services.AddTransient<MainViewModel>(sp =>
                {
                    var logger = sp.GetRequiredService<ILoggerService>();
                    return new MainViewModel(
                        sp.GetRequiredService<IDataService>(),
                        sp.GetRequiredService<INotificationService>(),
                        sp.GetRequiredService<IPatternDetectionService>(),
                        sp.GetRequiredService<MorningCheckInViewModel>(),
                        sp.GetRequiredService<EveningCheckInViewModel>(),
                        sp.GetRequiredService<WeeklyReviewViewModel>(),
                        sp.GetRequiredService<VisualizationViewModel>(),
                        sp.GetRequiredService<SettingsViewModel>(),
                        logger);
                });
                services.AddTransient<MorningCheckInViewModel>();
                services.AddTransient<EveningCheckInViewModel>();
                services.AddTransient<WeeklyReviewViewModel>();
                services.AddTransient<VisualizationViewModel>();
                services.AddTransient<SettingsViewModel>();

                // Views
                _logger?.LogDebug("Registering Views...");
                services.AddTransient<MainWindow>(sp =>
                {
                    var logger = sp.GetRequiredService<ILoggerService>();
                    return new MainWindow(
                        sp.GetRequiredService<MainViewModel>(),
                        logger);
                });

                _logger?.LogDebug("Service configuration completed");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error configuring services");
                throw;
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                _logger?.LogError(ex, "Unhandled exception in AppDomain");
                MessageBox.Show(
                    $"An unhandled error occurred:\n\n{ex.Message}\n\nCheck the log file for details.",
                    "Application Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger?.LogError(e.Exception, "Unhandled exception in dispatcher");
            MessageBox.Show(
                $"An error occurred:\n\n{e.Exception.Message}\n\nCheck the log file for details.",
                "Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            e.Handled = true; // Prevent app crash
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger?.LogInformation("Application shutting down...");
            _serviceProvider?.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}

