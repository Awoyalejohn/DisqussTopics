namespace DisqussTopics.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Tile { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Video { get; set; }

        public ICollection<DTUser>? Upvotes { get; set; } // navigation property
        public ICollection<DTUser>? Downvotes { get; set; } // navigation property
        public string DTUserId { get; set; }
        public DTUser Author { get; set; }


    }
}