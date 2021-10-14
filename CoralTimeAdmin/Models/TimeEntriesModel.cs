using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CoralTimeAdmin.Models
{
    public class TimeEntriesModel : BaseWebEntityModel
    {
        public TimeEntriesModel() {
            AvailableProjects = new List<SelectListItem>();
            AvailableTasks = new List<SelectListItem>();
        }
        public DateTime CreationDate { get; set; }
        public string CreatorId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsFromToShow { get; set; }
        public string LastEditorUserId { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int MemberId { get; set; }
        public int TimeEstimated { get; set; }
        public int ProjectId { get; set; }
        public int TaskTypesId { get; set; }
        public int TimeActual { get; set; }
        public int TimeFrom { get; set; }
        public int TimeTimerStart { get; set; }
        public int TimeTo { get; set; }

        public IList<SelectListItem> AvailableProjects { get; set; }
        public IList<SelectListItem> AvailableTasks { get; set; }
    }
}