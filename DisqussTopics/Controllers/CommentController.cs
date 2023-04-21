using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        public CommentController(ICommentRepository commentRepository, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Post,Comment")] PostDetailViewModel postDetailViewModel)
        {
            var postId = postDetailViewModel.Post.Id;
            var post = await _postRepository.GetPostById(postId);
            //var comment = postDetailViewModel.Comment;
            var slug = post.Slug;
            var topic = post.Topic.Name;

            // Get the current user id
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Comment()
            {
                Content = postDetailViewModel.Comment.Content,
                Created = postDetailViewModel.Comment.Created,
                Updated = postDetailViewModel.Comment.Updated,
                PostId = postId,
                DTUserId = currentUserId
            };

            if (ModelState.IsValid)
            {
               
                _commentRepository.InsertComment(comment);
                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
            }

            var newPostDetailViewModel = new PostDetailViewModel()
            {
                Post = post,
                Comment = comment,
            };

            if (!newPostDetailViewModel.Comment.Content.IsNullOrEmpty())
            {
                // Set the value of a session variable named "Content"
                HttpContext.Session.SetString("Content", comment.Content);
            }
            else
            {
                HttpContext.Session.SetString("NoContent", "Comment cannot be empty!");
            }

      

            return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
