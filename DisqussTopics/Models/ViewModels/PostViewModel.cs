using Microsoft.AspNetCore.Mvc.Rendering;

namespace DisqussTopics.Models.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; } = null!;
        public int TopicId { get; set; }
        public SelectList? Topics { get; set; }
        public string? DTUserId { get; set; }
        public string? URL { get; set; }
        public IFormFile? UploadImage { get; set; }
        public IFormFile? UploadVideo { get; set; }
    }
}
