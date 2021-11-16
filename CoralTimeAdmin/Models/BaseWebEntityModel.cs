using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace CoralTimeAdmin.Models
{
    public class BaseWebEntityModel
    {
        public int Id { get; set; }
    }

    public class ViewModelBase : BaseWebEntityModel
    {
        [NotMapped] public IList<SelectListItem> AvailableProjects { get; set; }
        [NotMapped] public IList<SelectListItem> AvailableTasks { get; set; }
    }
}