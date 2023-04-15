namespace DisqussTopics.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public ICollection<DTUser>? Upvotes { get; set; } // navigation property
        public ICollection<DTUser>? Downvotes { get; set; } // navigation property
        public DTUser? Commenter { get; set; } // naviagtion property
        public Post? Post { get; set; } // naviagtion property
    }
}