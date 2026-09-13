using System;

namespace SDTHMS.Models
{
    public class Goal
    {
        public int GoalId { get; set; }
        public int UserId { get; set; }
        public string GoalTitle { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; }

        public Goal(int userId, string goalTitle, DateTime targetDate, string status = "In Progress")
        {
            UserId = userId;
            GoalTitle = goalTitle;
            TargetDate = targetDate;
            Status = status;
        }

        public bool IsValid()
        {
            if (UserId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(GoalTitle))
                return false;

            if (string.IsNullOrWhiteSpace(Status))
                return false;

            return Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase) || Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
        }
        public bool IsOverdue()
        {
            return TargetDate.Date < DateTime.Now.Date && Status != "Completed";
        }
    }
}