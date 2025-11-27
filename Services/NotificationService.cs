using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DailyCheckInJournal.Services
{
    public class NotificationService : INotificationService
    {
        private Timer? _morningTimer;
        private Timer? _eveningTimer;
        private string? _morningTitle;
        private string? _morningMessage;
        private string? _eveningTitle;
        private string? _eveningMessage;

        public event EventHandler<string>? InAppNotificationReceived;

        public async Task ShowSystemNotificationAsync(string title, string message)
        {
            try
            {
                // Windows 10/11 Toast Notification
                var toastXml = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(
                    Windows.UI.Notifications.ToastTemplateType.ToastText02);

                var textNodes = toastXml.GetElementsByTagName("text");
                if (textNodes.Count > 0)
                    textNodes[0].AppendChild(toastXml.CreateTextNode(title));
                if (textNodes.Count > 1)
                    textNodes[1].AppendChild(toastXml.CreateTextNode(message));

                var toast = new Windows.UI.Notifications.ToastNotification(toastXml);
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier("DailyCheckInJournal").Show(toast);

                // Also trigger in-app notification
                InAppNotificationReceived?.Invoke(this, $"{title}: {message}");
            }
            catch
            {
                // Fallback to MessageBox if toast notifications fail
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                });
                InAppNotificationReceived?.Invoke(this, $"{title}: {message}");
            }
        }

        public Task ScheduleReminderAsync(TimeSpan time, string title, string message, bool isMorning)
        {
            if (isMorning)
            {
                _morningTitle = title;
                _morningMessage = message;
                ScheduleTimer(time, isMorning);
            }
            else
            {
                _eveningTitle = title;
                _eveningMessage = message;
                ScheduleTimer(time, isMorning);
            }

            return Task.CompletedTask;
        }

        private void ScheduleTimer(TimeSpan targetTime, bool isMorning)
        {
            var now = DateTime.Now;
            var targetDateTime = now.Date.Add(targetTime);
            
            if (targetDateTime <= now)
            {
                targetDateTime = targetDateTime.AddDays(1);
            }

            var delay = targetDateTime - now;

            var timer = new Timer(async _ =>
            {
                if (isMorning)
                {
                    await ShowSystemNotificationAsync(_morningTitle ?? "Morning Check-In", 
                        _morningMessage ?? "Time for your morning check-in!");
                    ScheduleTimer(targetTime, true); // Reschedule for next day
                }
                else
                {
                    await ShowSystemNotificationAsync(_eveningTitle ?? "Evening Check-In", 
                        _eveningMessage ?? "Time for your evening check-in!");
                    ScheduleTimer(targetTime, false); // Reschedule for next day
                }
            }, null, delay, Timeout.InfiniteTimeSpan);

            if (isMorning)
            {
                _morningTimer?.Dispose();
                _morningTimer = timer;
            }
            else
            {
                _eveningTimer?.Dispose();
                _eveningTimer = timer;
            }
        }

        public void CancelReminder(bool isMorning)
        {
            if (isMorning)
            {
                _morningTimer?.Dispose();
                _morningTimer = null;
            }
            else
            {
                _eveningTimer?.Dispose();
                _eveningTimer = null;
            }
        }
    }
}

