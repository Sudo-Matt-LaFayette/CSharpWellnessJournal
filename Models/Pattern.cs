using System;
using System.Collections.Generic;

namespace DailyCheckInJournal.Models
{
    public class Pattern
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PatternType Type { get; set; }
        public DateTime DetectedDate { get; set; } = DateTime.Now;
        public DateTime? LastNotified { get; set; }
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public enum PatternType
    {
        EnergyPattern,
        OvercommitmentPattern,
        MistakePattern,
        MoodPattern,
        SleepPattern,
        SensoryPattern,
        ExecutiveFunctionPattern,
        Custom
    }
}

