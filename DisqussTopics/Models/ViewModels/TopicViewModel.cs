﻿namespace DisqussTopics.Models.ViewModels
{
    public class TopicViewModel
    {
        public Topic Topic { get; set; } = null!;
        public string? DTUserId { get; set; }
        public IEnumerable<Post>? Posts { get; set; }
        public IFormFile? BannerUpload { get; set; }
        public IFormFile? IconUpload { get; set; }
    }
}
