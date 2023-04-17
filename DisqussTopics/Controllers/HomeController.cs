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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Post,TopicId,Topics")] PostViewModel postViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _postRepository.InsertPost(post);
        //        await _postRepository.SaveAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(post);
        //}

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
