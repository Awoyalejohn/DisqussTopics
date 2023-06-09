﻿using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using DisqussTopics.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slugify;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IImageService _imageService;

        public TopicController(ITopicRepository topicRepository, IUserRepository userRepository, IPostRepository postRepository, IImageService imageService)
        {
            _topicRepository = topicRepository;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _imageService = imageService;
        }

        // GET: Topic/Index
        public async Task<IActionResult> Index()
        {
            var topics = await _topicRepository.GetTopics();

            return View(topics);
        }

        // GET: Topic/Create
        [Authorize(Roles = "User, Admin")]
        public IActionResult Create()
        {
            var currenUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var topicViewModel = new TopicViewModel()
            {
                DTUserId = currenUserId,
            };
            return View(topicViewModel);
        }

        // POST Topic/Create
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Topic, DTUserId")] TopicViewModel topicViewModel)
        {
            if (ModelState.IsValid)
            {
                // instantiate SlugHelper class
                SlugHelper helper = new SlugHelper();

                var slug = helper.GenerateSlug(topicViewModel.Topic.Name);

                var topics = await _topicRepository.GetTopics();

                bool topicExists = topics.Any(t => t.Slug == slug);

                var currentUser = await _userRepository.GetUserByIdAsync(topicViewModel.DTUserId);

                if (topicExists)
                {
                    ModelState.AddModelError("", "Topic already exists!");
                    TempData["Error"] = "Failed to create Topic!";
                    return View(topicViewModel);
                }

                Topic topic = new Topic()
                {
                    Name = topicViewModel.Topic.Name,
                    Slug = slug,
                    Created = DateTime.Now,
                    About = topicViewModel.Topic.About,
                    Banner = string.Empty,
                    Icon = string.Empty,
                    DTUserId = topicViewModel.DTUserId,
                    DTUsers = new[] { currentUser },
                };

                // Subscribe topic creator to topic
                _topicRepository.InsertTopic(topic);
                currentUser.SubscibedTopics.Add(topic);

                await _topicRepository.SaveAsync();
                
                TempData["Success"] = "Topic created successfully!";
                return RedirectToAction("Detail", "Topic", new { slug = topic.Slug });
            }

            TempData["Error"] = "Failed to create Topic!";
            return View(topicViewModel);
        }

        // GET: Topic/Detail/{slug}
        public async Task<IActionResult> Detail(string slug)
        {
            var topic = await _topicRepository.GetTopicBySlugNoTrackng(slug);

            if (topic == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);


            var posts = topic.Posts.OrderByDescending(p => p.Votes);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
                DTUserId = currentUserId,
                Posts = posts
            };
            

            return View(topicViewModel);
        }

        // GET: Topic/MostUpvoted/{slug}
        public async Task<IActionResult> MostUpvoted(string slug)
        {
            var topic = await _topicRepository.GetTopicBySlugNoTrackng(slug);

            if (topic == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = topic.Posts.OrderByDescending(p => p.Votes);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
                DTUserId = currentUserId,
                Posts = posts
            };

            return View("Detail", topicViewModel);
        }

        // GET: Topic/MostDiscussed/{slug}
        public async Task<IActionResult> MostDiscussed(string slug)
        {
            var topic = await _topicRepository.GetTopicBySlugNoTrackng(slug);

            if (topic == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = topic.Posts
                .OrderByDescending(p => p.Comments?.Count ?? 0);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
                DTUserId = currentUserId,
                Posts = posts
            };

            return View("Detail", topicViewModel);
        }

        // GET: Topic/NewPost/{slug}
        public async Task<IActionResult> NewPosts(string slug)
        {
            var topic = await _topicRepository.GetTopicBySlugNoTrackng(slug);

            if (topic == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = topic.Posts
                .OrderByDescending(p => p.Created.Date)
                .ThenByDescending(p => p.Created.TimeOfDay);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
                DTUserId = currentUserId,
                Posts = posts
            };

            return View("Detail", topicViewModel);
        }


        // GET: Topic/Edit/{slug}
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Edit(string slug) 
        {
            var topic = await _topicRepository
                .GetTopicBySlugNoTrackng(slug);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
            };
            return View(topicViewModel);
        }

        // POST: Topic/Edit/{slug}
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string slug, [Bind("Topic,DTUserId,Posts,BannerUpload,IconUpload")] TopicViewModel topicViewModel)
        {
            if (ModelState.IsValid)
            {
                var topic = await _topicRepository
                    .GetTopicBySlug(slug);

                SlugHelper helper = new SlugHelper();
                string updateSlug = helper.GenerateSlug(topicViewModel.Topic.Name);

                var topics = await _topicRepository.GetTopics();
                // checks if the topic already exists
                bool topicExists = topics.Any(t => t.Slug == updateSlug);

                // only allows topic slug to be duplicated if it is the same topic being updated
                if (topicExists)
                {
                    var duplicateTopic = await _topicRepository
                        .GetTopicBySlug(updateSlug);

                    if (duplicateTopic.Id != topic.Id)
                    {
                        ModelState.AddModelError("", "Topic already exists!");
                        TempData["Error"] = "Failed to edit Topic!";
                        return View(topicViewModel);
                    }
                }

                // Get the Icon image result
                var iconImageResult = await _imageService.AddImageAsync(topicViewModel.IconUpload);
                string? iconImageResultURL = string.Empty;
                if (iconImageResult.SecureUrl != null)
                {
                    iconImageResultURL = iconImageResult.SecureUrl.ToString();

                    if (topic.Banner != null && topic.Banner.Length > 5)
                    {
                        // try to delete the old banner image
                        try
                        {
                            var fileInfo = new FileInfo(topic.Banner);
                            var publicId = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                            await _imageService.DeleteImageAsync(publicId);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "Failed to edit banner image!");
                            TempData["Error"] = "Failed to edit banner image!";
                            return View(topicViewModel);
                        }
                    }
                }
                else
                {
                    iconImageResultURL = topic.Icon;
                }

                // Get the Banner image result
                var bannerImageResult = await _imageService.AddImageAsync(topicViewModel.BannerUpload);
                string? bannerImageResultURL = string.Empty;
                if (bannerImageResult.SecureUrl != null)
                {
                    bannerImageResultURL = bannerImageResult.SecureUrl.ToString();

                    if (topic.Icon != null && topic.Icon.Length > 5)
                    {
                        // try to delete the old icon image
                        try
                        {
                            var fileInfo = new FileInfo(topic.Icon);
                            var publicId = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                            await _imageService.DeleteImageAsync(publicId);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "Failed to edit banner image!");
                            TempData["Error"] = "Failed to edit banner image!";
                            return View(topicViewModel);
                        }
                    }
                }
                else
                {
                    bannerImageResultURL = topic.Banner;
                }

                topic.Name = topicViewModel.Topic.Name;
                topic.Slug = updateSlug;
                topic.About = topicViewModel.Topic.About;
                topic.Icon = iconImageResultURL;
                topic.Banner = bannerImageResultURL;

                _topicRepository.UpdateTopic(topic);
                await _topicRepository.SaveAsync();

                TempData["Success"] = "Topic updated successfully!";
                return RedirectToAction("Detail", "Topic", new { slug = updateSlug });
            }

            TempData["Error"] = "Failed to edit Topic!";
            return View(topicViewModel);
        }

        // GET: Topic/Delete/{slug}
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Delete(string slug)
        {
            var topic = await _topicRepository
                .GetTopicBySlugNoTrackng(slug);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (topic == null) { return NotFound(); }

            if (!User.IsInRole("Admin")  && currentUserId != topic.DTUserId) 
            {
                return NotFound();
            }


            return View(topic);
        }

        // POST: Topic/Delete/{slug}
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string slug)
        {
            var topic = await _topicRepository
                .GetTopicBySlugAsync(slug);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (topic == null) { return NotFound(); }

            if (!User.IsInRole("Admin") && currentUserId != topic.DTUserId)
            {
                return NotFound();
            }

            _topicRepository.DeleteTopic(topic);
            await _topicRepository.SaveAsync();

            TempData["Success"] = "Topic deleted successfully!";
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string slug)
        {
            var topic = await _topicRepository
                .GetTopicBySlug(slug);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            topic.DTUsers.Add(currentUser);
            currentUser.SubscibedTopics.Add(topic);
            await _topicRepository.SaveAsync();

            TempData["Success"] = $"Subscribed to {topic.Name}!";
            return RedirectToAction("Detail", "Topic", new { Slug = slug });
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe(string slug)
        {
            var topic = await _topicRepository
                .GetTopicBySlug(slug);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userRepository
                .GetUserByIdAsync(currentUserId);

            topic.DTUsers.Remove(currentUser);
            currentUser.SubscibedTopics.Remove(topic);
            await _topicRepository.SaveAsync();

            TempData["Success"] = $"Unsubscribed from {topic.Name}!";
            return RedirectToAction("Detail", "Topic", new { Slug = slug });
        }
    }
}
