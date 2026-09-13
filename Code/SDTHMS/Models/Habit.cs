using System;

namespace SDTHMS.Models
{
    public class Habit
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public string HabitName { get; set; }
        public string Frequency { get; set; }
        public int StreakCount { get; set; }
        public DateTime? LastCompletedDate { get; set; }

        public Habit(int userId, string habitName, string frequency = "Daily")
        {
            UserId = userId;
            HabitName = habitName;
            Frequency = frequency;
            StreakCount = 0;
            LastCompletedDate = null;
        }

        public bool IsValid()
        {
            if (UserId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(HabitName))
                return false;

            if (string.IsNullOrWhiteSpace(Frequency))
                return false;

            return Frequency.Equals("Daily", StringComparison.OrdinalIgnoreCase)
                || Frequency.Equals("Weekly", StringComparison.OrdinalIgnoreCase)
                || Frequency.Equals("Monthly", StringComparison.OrdinalIgnoreCase);
        }

        public void CompleteHabit()
        {
            StreakCount++;
            LastCompletedDate = DateTime.Now;
        }
    }
}