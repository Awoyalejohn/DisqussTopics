using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DisqussTopics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ITopicRepository _topicRepository;

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository, ITopicRepository topicRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
            _topicRepository = topicRepository;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            return View( await _postRepository.GetPosts());
        }

        // GET: Home/Create
        public async Task<IActionResult> Create()
        {
            var postViewModel = new PostViewModel()
            {
                Topics = new SelectList(await _topicRepository.GetTopicsQuery().ToListAsync(), "Id", "Name") 
            };
            return View(postViewModel);
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Post,TopicId,Topics")] PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                // Get the topic Id form the form data
                var topicId = postViewModel.TopicId;

                // Map the postViewModel properties to the post model
                var post = new Post()
                {
                    TopicId = topicId,
                    Title = postViewModel.Post.Title,
                    Slug = postViewModel.Post.Slug,
                    Created = postViewModel.Post.Created,
                    Updated = postViewModel.Post.Updated,
                    Content = postViewModel.Post.Content,
                    Image = postViewModel.Post.Image,
                    Video = postViewModel.Post.Video,
                };
                _postRepository.InsertPost(post);
                await _postRepository.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, redisplay the form with validation errors
            postViewModel.Topics = new SelectList(await _topicRepository.GetTopicsQuery().ToListAsync(), "Id", "Name");
            return View(postViewModel);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
