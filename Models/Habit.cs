using System;

namespace DailyCheckInJournal.Models
{
    public class Habit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public HabitCategory Category { get; set; }
        public bool IsCustom { get; set; } = false;
    }

    public enum HabitCategory
    {
        Exercise,
        Nutrition,
        SelfCare,
        Social,
        Productivity,
        Sleep,
        Medication,
        Other
    }
}

