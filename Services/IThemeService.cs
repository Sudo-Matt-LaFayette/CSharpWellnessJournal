using System.Collections.Generic;

namespace DailyCheckInJournal.Services
{
    public interface IThemeService
    {
        List<string> GetAvailableThemes();
        void ApplyTheme(string themeName);
        string GetCurrentTheme();
    }
}

