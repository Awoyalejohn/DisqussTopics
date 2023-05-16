using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace DisqussTopics.Models
{
    public class DTUser : IdentityUser
    {
        [Required]
        public string? DTUsername { get; set; }
        public string? Bio { get; set; }
        public string? Avatar { get; set; }

        public ICollection<Post>? Posts { get; set; } // navigation property
        public ICollection<Comment>? Comments { get; set; } // navigation property
        public ICollection<Topic>? CreatedTopics { get; set; } // navigation property
        public ICollection<Topic>? SubscibedTopics { get; set; } // navigation property
        public ICollection<Post>? PostUpvotes{ get; set; } // navigation property
        public ICollection<Post>? PostDownvotes { get; set; } // navigation property
        public ICollection<Comment>? CommentUpvotes{ get; set; } // navigation property
        public ICollection<Comment>? CommentDownvotes{ get; set; } // navigation property
    }
}
