using System;

namespace SDTHMS.Models
{
    public class ManagerSuggestion
    {
        public int SuggestionId { get; set; }
        public int UserId { get; set; }
        public int ManagerId { get; set; }
        public string SuggestionText { get; set; }
        public DateTime CreatedAt { get; set; }

        public ManagerSuggestion(int userId, int managerId, string suggestionText)
        {
            UserId = userId;
            ManagerId = managerId;
            SuggestionText = suggestionText;
            CreatedAt = DateTime.Now;
        }

        public bool IsValid()
        {
            if (UserId <= 0) return false;

            if (ManagerId <= 0) return false;

            if (string.IsNullOrWhiteSpace(SuggestionText)) return false;

            return true;
        }
    }
}