using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorldTrackerWeb.Models
{
    public class VisitCreateViewModel
    {
        public List<SelectListItem> People { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name ="Person")]
        public string PersonID { get; set; }

        public List<SelectListItem> Places { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Place")]
        public string PlaceID { get; set; }
    }
}
