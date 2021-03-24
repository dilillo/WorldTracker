using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WorldTrackerWeb.Models
{
    public class PlaceCreateViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public IFormFile Picture { get; set; }
    }
}
