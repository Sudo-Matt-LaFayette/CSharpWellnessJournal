using System;
using System.Collections.Generic;

namespace DailyCheckInJournal.Models
{
    public class Goal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public GoalType Type { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? TargetDate { get; set; }
        public GoalStatus Status { get; set; } = GoalStatus.Active;
        public List<GoalProgressEntry> ProgressEntries { get; set; } = new();
    }

    public enum GoalType
    {
        ShortTerm, // Weekly
        LongTerm   // Monthly/Yearly
    }

    public enum GoalStatus
    {
        Active,
        Completed,
        Paused,
        Cancelled
    }

    public class GoalProgressEntry
    {
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public int ProgressValue { get; set; } // 0-100 or custom scale
    }
}

