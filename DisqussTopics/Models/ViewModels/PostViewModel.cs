using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models.ViewModels
{
    public class PostViewModel
    {
        [Required]
        public Post Post { get; set; } = null!;
        public int TopicId { get; set; }
        public SelectList? Topics { get; set; }
        public string? DTUserId { get; set; }
        public string? URL { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? UploadImage { get; set; }

        [Display(Name = "Upload Video")]
        public IFormFile? UploadVideo { get; set; }
    }
}
