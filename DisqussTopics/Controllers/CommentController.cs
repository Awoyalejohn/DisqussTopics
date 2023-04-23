using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    [Route("Post")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public CommentController(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        [Route("Detail/{topic}/{slug}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Detail")]
        public async Task<IActionResult> CreateComment([Bind("Post,Comment")] PostDetailViewModel postDetailViewModel)
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
                Created = DateTime.Now,
                Updated = DateTime.Now,
                PostId = postId,
                DTUserId = currentUserId
            };

            if (ModelState.IsValid)
            {
                _commentRepository.InsertComment(comment);
                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }

            var comments = await _commentRepository.GetPostCommentsNoTracking(post);


            var postUpvoted = post.Upvotes.Any(u => u.Id == currentUserId);
            var postDownvoted = post.Downvotes.Any(u => u.Id == currentUserId);

            var newPostDetailViewModel = new PostDetailViewModel()
            {
                Post = post,
                Comment = comment,
                PostUpvoted = postUpvoted,
                PostDownvoted = postDownvoted,
                Comments = comments,
                UserId = currentUserId
            };

            return View("~/Views/Post/Detail.cshtml", newPostDetailViewModel);
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpvoteComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var post = await _postRepository.GetPostById(comment.PostId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            if (!comment.Downvotes.Any(c => c.Id == currentUserId))
            {
                comment.Upvotes.Add(currentUser);
                currentUser.CommentUpvotes.Add(comment);
                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
            else
            {
                comment.Downvotes.Remove(currentUser);
                currentUser.CommentDownvotes.Remove(comment);

                comment.Upvotes.Add(currentUser);
                currentUser.CommentUpvotes.Add(comment);

                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUpvoteComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var post = await _postRepository.GetPostById(comment.PostId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            comment.Upvotes.Remove(currentUser);
            currentUser.CommentUpvotes.Remove(comment);
            await _commentRepository.SaveAsync();
            return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownvoteComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var post = await _postRepository.GetPostById(comment.PostId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            if (!comment.Upvotes.Any(c => c.Id == currentUserId))
            {
                comment.Downvotes.Add(currentUser);
                currentUser.CommentDownvotes.Add(comment);
                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
            else
            {
                comment.Upvotes.Remove(currentUser);
                currentUser.CommentUpvotes.Remove(comment);

                comment.Downvotes.Add(currentUser);
                currentUser.CommentDownvotes.Add(comment);
                await _commentRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDownvoteComment(int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var post = await _postRepository.GetPostById(comment.PostId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            comment.Downvotes.Remove(currentUser);
            currentUser.CommentDownvotes.Remove(comment);
            await _commentRepository.SaveAsync();
            return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
