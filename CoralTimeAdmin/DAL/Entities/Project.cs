using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTimeAdmin.DAL.Entities
{
    [Table("Projects")]
    public class Project : BaseEntity
    {
        public int? ClientId { get; set; }

        public int Color { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreatorId { get; set; }

        public int DaysBeforeStopEditTimeEntries { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsActiveBeforeArchiving { get; set; }

        public bool IsNotificationEnabled { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsTimeLockEnabled { get; set; }

        public string LastEditorUserId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public int LockPeriod { get; set; }

        public string Name { get; set; }

        public int NotificationDay { get; set; }
    }
}