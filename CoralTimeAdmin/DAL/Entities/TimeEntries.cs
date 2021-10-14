using System;

namespace CoralTimeAdmin.DAL.Entities
{
    public class TimeEntries
    {
        public int Id { get; set; }

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
    }
}