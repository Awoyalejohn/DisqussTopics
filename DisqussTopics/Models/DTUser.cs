namespace DisqussTopics.Models
{
    public class DTUser
    {
        public int Id { get; set; }
        public string? DTUsername { get; set; }
        public string? Bio { get; set; }
        public string? Avatar { get; set; }

        public ICollection<Post>? Posts { get; set; } // navigation property
        public ICollection<Comment>? Comments { get; set; } // navigation property
        public ICollection<Topic>? CreatedTopics { get; set; } // navigation property
        public ICollection<Topic>? SubscibedTopics { get; set; } // navigation property
    }
}
