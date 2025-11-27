using System;
using System.Threading.Tasks;

namespace DailyCheckInJournal.Services
{
    public interface INotificationService
    {
        Task ShowSystemNotificationAsync(string title, string message);
        Task ScheduleReminderAsync(TimeSpan time, string title, string message, bool isMorning);
        void CancelReminder(bool isMorning);
        event EventHandler<string>? InAppNotificationReceived;
    }
}

