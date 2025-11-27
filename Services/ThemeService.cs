using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DailyCheckInJournal.Models;

namespace DailyCheckInJournal.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IDataService _dataService;
        private string _currentTheme = "Default";

        public ThemeService(IDataService dataService)
        {
            _dataService = dataService;
            LoadCurrentTheme();
        }

        private async void LoadCurrentTheme()
        {
            var settings = await _dataService.GetSettingsAsync();
            _currentTheme = settings.CurrentTheme;
        }

        public List<string> GetAvailableThemes()
        {
            return new List<string> { "Default", "Dark", "Light", "High Contrast", "Autumn", "Ocean" };
        }

        public async void ApplyTheme(string themeName)
        {
            _currentTheme = themeName;
            
            var settings = await _dataService.GetSettingsAsync();
            settings.CurrentTheme = themeName;
            await _dataService.SaveSettingsAsync(settings);

            // Apply theme resources
            var app = Application.Current;
            var themeDict = new ResourceDictionary();
            
            switch (themeName)
            {
                case "Dark":
                    themeDict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
                case "Light":
                    themeDict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                    break;
                case "High Contrast":
                    themeDict.Source = new Uri("Themes/HighContrastTheme.xaml", UriKind.Relative);
                    break;
                case "Autumn":
                    themeDict.Source = new Uri("Themes/AutumnTheme.xaml", UriKind.Relative);
                    break;
                case "Ocean":
                    themeDict.Source = new Uri("Themes/OceanTheme.xaml", UriKind.Relative);
                    break;
                default:
                    themeDict.Source = new Uri("Themes/DefaultTheme.xaml", UriKind.Relative);
                    break;
            }

            app.Resources.MergedDictionaries.Clear();
            app.Resources.MergedDictionaries.Add(themeDict);
        }

        public string GetCurrentTheme()
        {
            return _currentTheme;
        }
    }
}

