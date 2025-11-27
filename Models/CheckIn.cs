using System;
using System.Collections.Generic;

namespace DailyCheckInJournal.Models
{
    public class CheckIn
    {
        public DateTime Date { get; set; }
        public MorningCheckIn? Morning { get; set; }
        public EveningCheckIn? Evening { get; set; }
    }

    public class MorningCheckIn
    {
        public int EnergyLevel { get; set; } // 1-10
        public string? MustDoToday { get; set; }
        public int CapacityFeeling { get; set; } // 1-10 scale
        public SleepQuality? SleepQuality { get; set; }
        public List<MedicationEntry> Medications { get; set; } = new();
        public SensoryState? SensoryState { get; set; }
        public ExecutiveFunctionState? ExecutiveFunction { get; set; }
        public EmotionalState? EmotionalState { get; set; }
        public List<string> GratitudeItems { get; set; } = new();
        public List<TriggerEntry> Triggers { get; set; } = new();
        public List<CopingStrategy> CopingStrategies { get; set; } = new();
        public List<HabitEntry> HabitEntries { get; set; } = new();
        public DateTime CheckInTime { get; set; } = DateTime.Now;
    }

    public class EveningCheckIn
    {
        public bool? MustDoCompleted { get; set; }
        public int EnergyLevel { get; set; } // 1-10
        public bool? Overcommitted { get; set; }
        public List<MistakeEntry> CommonMistakes { get; set; } = new();
        public EmotionalState? EmotionalState { get; set; }
        public List<string> GratitudeItems { get; set; } = new();
        public List<TriggerEntry> Triggers { get; set; } = new();
        public List<CopingStrategy> CopingStrategies { get; set; } = new();
        public List<HabitEntry> HabitEntries { get; set; } = new();
        public string? Notes { get; set; }
        public DateTime CheckInTime { get; set; } = DateTime.Now;
    }

    public class SleepQuality
    {
        public int Quality { get; set; } // 1-10
        public int HoursSlept { get; set; }
        public bool Restless { get; set; }
        public string? Notes { get; set; }
    }

    public class MedicationEntry
    {
        public string Name { get; set; } = string.Empty;
        public DateTime TimeTaken { get; set; }
        public double? Dosage { get; set; }
        public string? Notes { get; set; }
    }

    public class SensoryState
    {
        public int OverloadLevel { get; set; } // 0-10, 0 = no issues, 10 = severe overload
        public List<string> Triggers { get; set; } = new(); // noise, lighting, crowds, textures, etc.
        public string? Notes { get; set; }
    }

    public class ExecutiveFunctionState
    {
        public int TaskInitiationDifficulty { get; set; } // 0-10
        public int TransitionDifficulty { get; set; } // 0-10
        public bool TimeBlindness { get; set; }
        public bool HyperfocusPeriod { get; set; }
        public string? Notes { get; set; }
    }

    public class EmotionalState
    {
        public int OverallMood { get; set; } // 1-10
        public bool Meltdown { get; set; }
        public bool Shutdown { get; set; }
        public int OverwhelmLevel { get; set; } // 0-10
        public string? Notes { get; set; }
    }

    public class TriggerEntry
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // sensory, social, task-related, etc.
        public string? Notes { get; set; }
    }

    public class CopingStrategy
    {
        public string Strategy { get; set; } = string.Empty;
        public bool Effective { get; set; }
        public string? Notes { get; set; }
    }

    public class HabitEntry
    {
        public string HabitName { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public string? Notes { get; set; }
    }

    public class MistakeEntry
    {
        public string Category { get; set; } = string.Empty; // Overcommitted, SkippedMeals, NoBreaks, SensoryOverload, etc.
        public List<string> Tags { get; set; } = new();
        public string? FreeTextNotes { get; set; }
    }
}

