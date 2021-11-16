using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTimeAdmin.Models
{
    public class DayTasks : ViewModelBase
    {
        public int Id { get; set; }
        public string Project { get; set; }
        public int ProjectId { get; set; }
        public int TaskTypesId { get; set; }
        public string Task { get; set; }
        public string TaskDescription { get; set; }
        public string Date { get; set; }
        public string LastUpdateDate { get; set; }
        public string Description { get; set; }
        public int TimeActual { get; set; }
        public int TimeFrom { get; set; }
        public int TimeTo { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public int TimeTimerStart { get; set; }
        public string TimeStart { get; set; }
        public string Duration { get; set; }

        [NotMapped] public string EventStart { get; set; }
        [NotMapped] public string EventEnd { get; set; }
    }
}