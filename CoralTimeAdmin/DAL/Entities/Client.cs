using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTimeAdmin.DAL.Entities
{
    [Table("Clients")]
    public class Client : BaseEntity
    {
        public DateTime CreationDate { get; set; }

        public string CreatorId { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public string LastEditorUserId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string Name { get; set; }
    }
}