﻿using DisqussTopics.Constants;
using DisqussTopics.Data;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        public PostController(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

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
                return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });

            }
            else
            {
                post.Downvotes.Remove(currentUser);
                currentUser.PostDownvotes.Remove(post);

                post.Upvotes.Add(currentUser);
                currentUser.PostUpvotes.Add(post);

                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

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
            return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
        }


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
                return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
            }
            else
            {
                post.Upvotes.Remove(currentUser);
                currentUser.PostUpvotes.Remove(post);

                post.Downvotes.Add(currentUser);
                currentUser.PostDownvotes.Add(post);
                await _postRepository.SaveAsync();
                return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
            }
        }

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
            return RedirectToAction("Detail", "Home", new { Topic = topic, Slug = slug, Id = postId });
        }
    }
}
