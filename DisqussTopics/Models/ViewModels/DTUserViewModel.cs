using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models.ViewModels
{
    public class DTUserViewModel
    {
        [Display(Name = "Username")]
        public string? DTUsername { get; set; }
        public string? Bio { get; set; }

        [Display(Name = "Avatar Upload")]
        public IFormFile? AvatarUpload { get; set; }
    }
}
