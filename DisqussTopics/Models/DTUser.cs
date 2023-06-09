﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DisqussTopics.Models
{
    public class DTUser : IdentityUser
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter a Username!")]
        [Display(Name = "Username")]
        public string? DTUsername { get; set; }

        [StringLength(250)]
        public string? Bio { get; set; }

        [StringLength(250)]
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
