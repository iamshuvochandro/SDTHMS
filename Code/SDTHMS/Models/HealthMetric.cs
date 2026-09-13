using System;
using System.Globalization;

namespace SDTHMS.Models
{
    public class HealthMetric
    {
        public int MetricId { get; set; }
        public int UserId { get; set; }
        public string WaterIntake { get; set; }
        public int SleepHours { get; set; }
        public int TimeBasis { get; set; }
        public DateTime MetricDate { get; set; }

        public HealthMetric(int userId, string waterIntake, int sleepHours, int timeBasis)
        {
            UserId = userId;
            WaterIntake = waterIntake;
            SleepHours = sleepHours;
            TimeBasis = timeBasis;
            MetricDate = DateTime.Now;
        }

        public bool IsValid()
        {
            decimal waterAmount;

            bool validWater = !string.IsNullOrWhiteSpace(WaterIntake) && decimal.TryParse(WaterIntake.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out waterAmount) && waterAmount >= 0;

            return UserId > 0 && validWater && SleepHours >= 1 && SleepHours <= 8 && TimeBasis >= 1 && TimeBasis <= 30;
        }
    }
}