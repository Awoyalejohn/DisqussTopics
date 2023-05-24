using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string? Content { get; set; }

        [StringLength(250)]
        public string? Image { get; set; }

        [StringLength(250)]
        public string? Video { get; set; }
        public int Votes { get; set; }

        public ICollection<DTUser>? Upvotes { get; set; } // navigation property
        public ICollection<DTUser>? Downvotes { get; set; } // navigation property
        public string? DTUserId { get; set; } // foreign key property
        public DTUser? DTUser { get; set; } // navigation property
        public int TopicId { get; set; } // foreign key property
        public Topic? Topic { get; set; } // navitgation property
        public ICollection<Comment>? Comments { get; set; } // navigation property
    }
}