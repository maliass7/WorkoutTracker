using System.Collections.Generic;
using System.Diagnostics;

namespace WorkoutTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public int WorkoutProgramId { get; set; }
        public WorkoutProgram? WorkoutProgram { get; set; }

        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}