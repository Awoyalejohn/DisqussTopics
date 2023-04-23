﻿using DisqussTopics.Constants;
using DisqussTopics.Data;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Authorization;
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
                return RedirectToAction("Index", "Home");
            }

            // If the model state is not valid, redisplay the form with validation errors
            postViewModel.Topics = new SelectList(await _topicRepository
                .GetSubscribedTopics(currentUserId), "Id", "Name");
            return View(postViewModel);
        }

        //GET Home/{Topic}/{Slug}/{Id}
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);

            if (post == null) return NotFound();

            var comments = await _commentRepository.GetPostCommentsNoTracking(post);

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

            if (HttpContext.Session.GetString("Content") != null)
            {
                // Get view data from the session variable
                var content = HttpContext.Session.GetString("Content");
                postDetailViewModel.Comment.Content = content ?? string.Empty;

                // Remove the session variable to prevent it from being used again
                HttpContext.Session.Remove("Content");
            }
            else if (HttpContext.Session.GetString("NoContent") != null)
            {
                // Get view data from the session variable
                var noContent = HttpContext.Session.GetString("NoContent");
                ModelState.AddModelError("", noContent ?? "comment cannnot be empty");

                // Remove the session variable to prevent it from being used again
                HttpContext.Session.Remove("NoContent");
            }

            return View(postDetailViewModel);
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
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });

            }
            else
            {
                post.Downvotes.Remove(currentUser);
                currentUser.PostDownvotes.Remove(post);

                post.Upvotes.Add(currentUser);
                currentUser.PostUpvotes.Add(post);

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
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
            }
            else
            {
                post.Upvotes.Remove(currentUser);
                currentUser.PostUpvotes.Remove(post);

                post.Downvotes.Add(currentUser);
                currentUser.PostDownvotes.Add(post);
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
            await _postRepository.SaveAsync();
            return RedirectToAction("Detail", "Post", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
