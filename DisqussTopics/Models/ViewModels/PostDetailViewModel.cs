namespace DisqussTopics.Models.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; } = null!;
        public Comment Comment { get; set; } = null!;
        public IEnumerable<Comment>? Comments { get; set; }
    }
}
