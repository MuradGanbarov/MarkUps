using MarkUps.Models;
using System.ComponentModel.DataAnnotations;

namespace MarkUps.Areas.Admin.ViewModels
{
    public class ChefCreateVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name can contain minimum 3 characters")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        [MinLength(3, ErrorMessage = "Surname can contain minimum 3 characters")]
        [MaxLength(25, ErrorMessage = "Surname can contain maximum 25 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage ="Photo is required")]
        public IFormFile Photo { get; set; }
        public int? PositionId { get; set; }
        public List<Position>? Positions { get; set; }
    }
}
