namespace DisqussTopics.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Post> Posts { get; set; } = null!;
        public string? CurrentUserId { get; set; }
    }
}
