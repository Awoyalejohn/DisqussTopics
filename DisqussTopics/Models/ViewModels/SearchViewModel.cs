namespace DisqussTopics.Models.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<Post> Posts { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = null!;
        public IEnumerable<Topic> Topics { get; set; } = null!;
        public string? CurrentUserId { get; set; }
    }
}
