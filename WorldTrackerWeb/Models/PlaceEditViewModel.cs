using System.ComponentModel.DataAnnotations;

namespace WorldTrackerWeb.Models
{
    public class PlaceEditViewModel
    {
        [Required]
        [MaxLength(50)]
        public string ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
