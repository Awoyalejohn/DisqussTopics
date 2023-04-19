using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Slugify;

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
            return View(await _postRepository.GetPostsNoTracking());
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
        public async Task<IActionResult> Create([Bind("Post,TopicId,Topics,DTUserId")] PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                // Get the topic Id from the form data
                var topicId = postViewModel.TopicId;

                // instantiate SlugHelper class
                SlugHelper helper = new SlugHelper();

                var slug = helper.GenerateSlug(postViewModel.Post.Title);

                // Get the user Id 
                var currentUserId = HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                // Map the postViewModel properties to the post model
                var post = new Post()
                {
                    TopicId = topicId,
                    DTUserId = currentUserId,
                    Title = postViewModel.Post.Title,
                    Slug = slug,
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

        //GET Home/{Topic}/{Slug}/{Id}
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postRepository
                .GetPostByIdNoTracking(id);

            if (post == null) return NotFound();

            return View(post);
        }
        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
