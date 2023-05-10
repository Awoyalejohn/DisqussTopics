using DisqussTopics.Constants;
using DisqussTopics.Data;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Slugify;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ICommentRepository _commentRepository;
        public PostController(IPostRepository postRepository,
            IUserRepository userRepository,
            ITopicRepository topicRepository,
            ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _topicRepository = topicRepository;
            _commentRepository = commentRepository;
        }

        // GET: Post/Create
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Create()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var postViewModel = new PostViewModel()
            {
                Topics = new SelectList(await _topicRepository
                .GetSubscribedTopics(currentUserId), "Id", "Name")
            };
            return View(postViewModel);
        }

        // POST: Post/Create
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Post,TopicId,Topics,DTUserId")] PostViewModel postViewModel)
        {
            // Get the user Id 
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                // Get the topic Id from the form data
                var topicId = postViewModel.TopicId;

                // instantiate SlugHelper class
                SlugHelper helper = new SlugHelper();

                var slug = helper.GenerateSlug(postViewModel.Post.Title);



                // Map the postViewModel properties to the post model
                var post = new Post()
                {
                    TopicId = topicId,
                    DTUserId = currentUserId,
                    Title = postViewModel.Post.Title,
                    Slug = slug,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Content = postViewModel.Post.Content,
                    Image = postViewModel.Post.Image,
                    Video = postViewModel.Post.Video,
                };
                _postRepository.InsertPost(post);
                await _postRepository.SaveAsync();
                TempData["Success"] = "Post created successfully";
                return RedirectToAction("Index", "Home");
            }

            // If the model state is not valid, redisplay the form with validation errors
            postViewModel.Topics = new SelectList(await _topicRepository
                .GetSubscribedTopics(currentUserId), "Id", "Name");
            TempData["Error"] = "Error with form";
            return View(postViewModel);
        }

        //GET Post/{Topic}/{Slug}/{Id}
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);

            if (post == null) return NotFound();

            var comments = await _commentRepository.GetPostComments(post);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);


            var postUpvoted = post.Upvotes.Any(u => u.Id == currentUserId);
            var postDownvoted = post.Downvotes.Any(u => u.Id == currentUserId);
            //var commentUpvoted;
            //var commentDownvoted;

            var postDetailViewModel = new PostDetailViewModel()
            {
                Post = post,
                Comment = new Comment(),
                Comments = comments,
                PostUpvoted = postUpvoted,
                PostDownvoted = postDownvoted,
                UserId = currentUserId,
            };

            return View(postDetailViewModel);
        }

        // GET: Post/Edit/{topic}/{slug}/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);

            if (post == null) return NotFound();

            var topicId = post.TopicId;

            var currentUserId = HttpContext.User
              .FindFirstValue(ClaimTypes.NameIdentifier);

            var postViewModel = new PostViewModel()
            {
                Post = post,
                TopicId = topicId,
                Topics = new SelectList(await _topicRepository
                .GetSubscribedTopics(currentUserId), "Id", "Name"),
                DTUserId = currentUserId,
            };

            return View(postViewModel);
        }

        // POST: Post/Edit/{topic}/{slug}/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Post,TopicId,Topics,DTUserId")] PostViewModel postViewModel)
        {

            if (ModelState.IsValid)
            {
                var post = await _postRepository
                    .GetPostById(id);

                if (post == null) return NotFound();

                var topic = await _topicRepository
                    .GetTopicById(post.TopicId);

                if (topic == null) return NotFound();

                SlugHelper helper = new SlugHelper();
                string slug = helper.GenerateSlug(postViewModel.Post.Title);

                post.Title = postViewModel.Post.Title;
                post.Slug = slug;
                post.Updated = DateTime.Now;
                post.Content = postViewModel.Post.Content;
                post.Image = postViewModel.Post.Image;
                post.Video = postViewModel.Post.Video;

                _postRepository.UpdatePost(post);
                await _postRepository.SaveAsync();
             
                return RedirectToAction(nameof(Detail), new { Topic = topic.Name, Slug = slug, Id= post.Id });
            }

            return View(postViewModel);
        }

        // GET Post/Delete/{topic}/{slug}/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);

            if (post == null) return NotFound();

            return View(post);
        }

        // POST Post/Delete/{topic}/{slug}/{id}
        [ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _postRepository
                .GetPostById(id);

            if (post == null) return NotFound();

            _postRepository.DeletePost(post);
            await _postRepository.SaveAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Post/Share/Post/{id}
        [Route("{controller=Post}/{action=Share}/{id}")]
        public async Task<IActionResult> Share(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);
            var topic = post.Topic.Name;
            var postSlug = post.Slug;
            var url = @$"{HttpContext.Request.Host}/Post/Detail/{topic}/{postSlug}/{id}";

            var postViewModel = new PostViewModel()
            {
                Post = post,
                URL = url,
            };

            return PartialView("_SharePostPartial", postViewModel);
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpvotePost(int id)
        {
            var post = await _postRepository.GetPostById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            if (!post.Downvotes.Any(p => p.Id == currentUserId))
            {
                post.Upvotes.Add(currentUser);
                currentUser.PostUpvotes.Add(post);

                // count to total votes
                post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });

            }
            else
            {
                post.Downvotes.Remove(currentUser);
                currentUser.PostDownvotes.Remove(post);

                post.Upvotes.Add(currentUser);
                currentUser.PostUpvotes.Add(post);

                // count to total votes
                post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);

                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUpvotePost(int id)
        {
            var post = await _postRepository.GetPostById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            post.Upvotes.Remove(currentUser);
            currentUser.PostUpvotes.Remove(post);

            // count to total votes
            post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);
            await _postRepository.SaveAsync();
            return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownvotePost(int id)
        {
            var post = await _postRepository.GetPostById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            if (!post.Upvotes.Any(p => p.Id == currentUserId))
            {
                post.Downvotes.Add(currentUser);
                currentUser.PostDownvotes.Add(post);

                // count to total votes
                post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
            else
            {
                post.Upvotes.Remove(currentUser);
                currentUser.PostUpvotes.Remove(post);

                post.Downvotes.Add(currentUser);
                currentUser.PostDownvotes.Add(post);

                // count to total votes
                post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

        [Route("{action}/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDownvotePost(int id)
        {
            var post = await _postRepository.GetPostById(id);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            var topic = post.Topic.Name;
            var slug = post.Slug;
            var postId = post.Id;

            post.Downvotes.Remove(currentUser);
            currentUser.PostDownvotes.Remove(post);

            // count to total votes
            post.Votes = (post.Upvotes?.Count ?? 0) - (post.Downvotes?.Count ?? 0);
            await _postRepository.SaveAsync();
            return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
