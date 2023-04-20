using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;

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

            var comment = new Comment()
            {
                Content = postDetailViewModel.Comment.Content,
                Created = postDetailViewModel.Comment.Created,
                Updated = postDetailViewModel.Comment.Updated,
                PostId = postId,
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

            // Set the value of a session variable named "Content"
            HttpContext.Session.SetString("Content", comment.Content);

            //return View("~/Views/Home/Detail.cshtml", newPostDetailViewModel);
            return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
