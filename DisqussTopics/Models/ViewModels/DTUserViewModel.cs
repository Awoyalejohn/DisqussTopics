using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models.ViewModels
{
    public class DTUserViewModel
    {
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string? DTUsername { get; set; }

        [StringLength(250)]
        public string? Bio { get; set; }

        [Display(Name = "Avatar Upload")]
        public IFormFile? AvatarUpload { get; set; }
    }
}
