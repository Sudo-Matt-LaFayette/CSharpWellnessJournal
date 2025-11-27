using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DailyCheckInJournal.Controls
{
    public partial class TimePicker : UserControl
    {
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan), typeof(TimePicker),
                new PropertyMetadata(TimeSpan.Zero, OnSelectedTimeChanged));

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public TimePicker()
        {
            InitializeComponent();
            
            // Populate hours
            for (int i = 0; i < 24; i++)
            {
                HourComboBox.Items.Add(i);
            }

            // Populate minutes
            for (int i = 0; i < 60; i += 5)
            {
                MinuteComboBox.Items.Add(i);
            }

            HourComboBox.SelectionChanged += (s, e) => UpdateTime();
            MinuteComboBox.SelectionChanged += (s, e) => UpdateTime();
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimePicker picker && e.NewValue is TimeSpan time)
            {
                picker.HourComboBox.SelectedItem = time.Hours;
                picker.MinuteComboBox.SelectedItem = (time.Minutes / 5) * 5; // Round to nearest 5
            }
        }

        private void UpdateTime()
        {
            if (HourComboBox.SelectedItem is int hours && MinuteComboBox.SelectedItem is int minutes)
            {
                SelectedTime = new TimeSpan(hours, minutes, 0);
            }
        }
    }
}

