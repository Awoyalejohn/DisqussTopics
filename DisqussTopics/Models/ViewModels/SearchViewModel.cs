namespace DisqussTopics.Models.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
    }
}
