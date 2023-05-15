using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models.ViewModels
{
    public class TopicViewModel
    {
        public Topic Topic { get; set; } = null!;
        public string? DTUserId { get; set; }
        public IEnumerable<Post>? Posts { get; set; }

        [Display(Name = "Banner Upload")]
        public IFormFile? BannerUpload { get; set; }

        [Display(Name = "Icon Upload")]
        public IFormFile? IconUpload { get; set; }
    }
}
