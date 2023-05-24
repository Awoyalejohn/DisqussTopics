using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models
{
    public class Topic
    {
        public int Id { get; set; } // Primary key

        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Slug { get; set; }
        public DateTime Created { get; set; }

        [StringLength(250)]
        public string? About { get; set; }

        [StringLength(250)]
        public string? Banner { get; set; }

        [StringLength(250)]
        public string? Icon { get; set; }

        public string? DTUserId { get; set; } // foreign key property
        public DTUser? DTUser { get; set; } // navigation property
        public ICollection<Post>? Posts { get; set; } // naviagtion property
        public ICollection<DTUser>? DTUsers { get; set; } // navigation property
        public ICollection<Rule>? Rules { get; set; } // navigation property
    }
}
