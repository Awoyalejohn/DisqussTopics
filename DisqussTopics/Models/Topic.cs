namespace DisqussTopics.Models
{
    public class Topic
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public DateTime Created { get; set; }
        public string? About { get; set; }
        public string? Banner { get; set; }
        public string? Icon { get; set; }

        public string? DTUserId { get; set; } // foreign key property
        public DTUser? DTUser { get; set; } // navigation property
        public ICollection<Post>? Posts { get; set; } // naviagtion property
        public ICollection<DTUser>? DTUsers { get; set; } // navigation property
        public ICollection<Rule>? Rules { get; set; } // navigation property
    }
}
