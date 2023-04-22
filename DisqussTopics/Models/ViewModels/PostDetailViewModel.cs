namespace DisqussTopics.Models.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; } = null!;
        public Comment Comment { get; set; } = null!;
        public IEnumerable<Comment>? Comments { get; set; }
        public bool PostUpvoted { get; set; }
        public bool PostDownvoted { get; set; }
        public string? UserId { get; set; }
    }
}
