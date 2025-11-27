using System;
using System.Collections.Generic;

namespace DailyCheckInJournal.Models
{
    public class AppSettings
    {
        public string? PasswordHash { get; set; }
        public bool RequirePassword { get; set; } = false;
        public TimeSpan MorningReminderTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan EveningReminderTime { get; set; } = new TimeSpan(20, 0, 0);
        public bool EnableSystemNotifications { get; set; } = true;
        public bool EnableInAppNotifications { get; set; } = true;
        public string CurrentTheme { get; set; } = "Default";
        public bool AutoSyncToGoogleDrive { get; set; } = false;
        public string? GoogleDriveAccessToken { get; set; }
        public string? GoogleDriveRefreshToken { get; set; }
        public List<string> CustomHabits { get; set; } = new();
        public List<string> MistakeCategories { get; set; } = new()
        {
            "Overcommitted",
            "Skipped Meals",
            "No Breaks",
            "Sensory Overload",
            "Time Blindness",
            "Task Avoidance",
            "Social Overwhelm",
            "Executive Function Difficulty",
            "Emotional Dysregulation",
            "Other"
        };
    }
}

