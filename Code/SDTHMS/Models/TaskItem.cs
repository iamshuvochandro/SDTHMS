using System;

namespace SDTHMS.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string TaskName { get; set; }
        public string Category { get; set; }
        public DateTime ScheduleTime { get; set; }
        public bool HasAlarm { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public TaskItem(int userId, string taskName, string category, DateTime scheduleTime, bool hasAlarm)
        {
            UserId = userId;
            TaskName = taskName;
            Category = category;
            ScheduleTime = scheduleTime;
            HasAlarm = hasAlarm;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }

        public bool IsValid()
        {
            if (UserId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(TaskName))
                return false;

            if (string.IsNullOrWhiteSpace(Category))
                return false;

            return Category.Equals("Work", StringComparison.OrdinalIgnoreCase) || Category.Equals("Personal", StringComparison.OrdinalIgnoreCase) || Category.Equals("Study", StringComparison.OrdinalIgnoreCase) || Category.Equals("General", StringComparison.OrdinalIgnoreCase);
        }
    }
}