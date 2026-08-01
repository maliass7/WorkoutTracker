using System;

namespace WorkoutTracker.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string Notes { get; set; } = string.Empty;

        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
    }
}