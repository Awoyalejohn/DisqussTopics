using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Slugify;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IUserRepository _userRepository;

        public TopicController(ITopicRepository topicRepository, IUserRepository userRepository)
        {
            _topicRepository = topicRepository;
            _userRepository = userRepository;
        }

        // GET: Topic/Create
        [Authorize]
        public IActionResult Create()
        {
            var currenUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var topicViewModel = new TopicViewModel()
            {
                DTUserId = currenUserId
            };
            return View(topicViewModel);
        }

        // POST Topic/Create
        [Authorize]
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

                if (topicExists)
                {
                    ModelState.AddModelError("", "Topic already exists!");
                    return View(topicViewModel);
                }

                Topic topic = new Topic()
                {
                    Name = topicViewModel.Topic.Name,
                    Slug = slug,
                    Created = DateTime.Now,
                    About = topicViewModel.Topic.About,
                    Banner = topicViewModel.Topic.Banner,
                    Icon = topicViewModel.Topic.Icon,
                    DTUserId = topicViewModel.DTUserId
                };

                _topicRepository.InsertTopic(topic);
                await _topicRepository.SaveAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(topicViewModel);
        }

        // GET: Topic/Detail/{slug}
        public async Task<IActionResult> Detail(string slug)
        {
            var topic = await _topicRepository.GetTopicBySlugNoTrackng(slug);

            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var topicViewModel = new TopicViewModel()
            {
                Topic = topic,
                DTUserId = currentUserId
            };
            if (topic == null)
            {
                return NotFound();
            }

            return View(topicViewModel);
        }

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

            return RedirectToAction("Detail", "Topic", new { Slug = slug });
        }

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

            return RedirectToAction("Detail", "Topic", new { Slug = slug });
        }
    }
}
