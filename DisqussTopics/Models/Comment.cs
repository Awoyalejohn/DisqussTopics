namespace DisqussTopics.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Votes { get; set; }

        public ICollection<DTUser>? Upvotes { get; set; } // navigation property
        public ICollection<DTUser>? Downvotes { get; set; } // navigation property
        public string? DTUserId { get; set; } // foreign key property
        public DTUser? DTUser { get; set; } // naviagtion property
        public int PostId { get; set; } // foreign key property
        public Post? Post { get; set; } // naviagtion property
    }
}