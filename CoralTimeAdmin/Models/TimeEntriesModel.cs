using System;
using System.ComponentModel.DataAnnotations;
using CoralTimeAdmin.DAL.Entities;

namespace CoralTimeAdmin.Models
{
    public class TimeEntriesModel : ViewModelBase
    {
        public DateTime CreationDate { get; set; }

        public string CreatorId { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public bool IsFromToShow { get; set; }

        public string LastEditorUserId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public int MemberId { get; set; }

        public int TimeEstimated { get; set; }

        [Display(Name = "Project", AutoGenerateFilter = false)]
        public int ProjectId { get; set; }

        [Display(Name = "Task", AutoGenerateFilter = false)]
        public int TaskTypesId { get; set; }

        public int TimeActual { get; set; }

        public int TimeFrom { get; set; }

        public int TimeTimerStart { get; set; }

        public int TimeTo { get; set; }

        [Display(Name = "From Time", AutoGenerateFilter = false)]
        public string TimeFromStr { get; set; }

        [Display(Name = "To Time", AutoGenerateFilter = false)]
        public string TimeToStr { get; set; }

        public DayTasks PreviewsTask { get; set; }

        public DayTasks NextTask { get; set; }
    }
}