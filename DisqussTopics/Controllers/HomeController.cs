using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Slugify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DisqussTopics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = await _postRepository.GetPosts();


            //if (User.Identity.IsAuthenticated)
            //{
            //    posts = posts
            //        .Where(p => p.Topic.DTUsers
            //        .Any(u => u.Id == currentUserId));
            //}

            //if (!string.IsNullOrEmpty(mostUpvoted))
            //{
            //    posts = posts
            //        .OrderBy(p => p.Upvotes?.Count ?? 0);
            //}

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
                CurrentUserId = currentUserId
            };

            return View(homeViewModel);
        }

        // GET: MostUpvoted
        public async Task<IActionResult> MostUpvoted()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = await _postRepository.GetPosts();

            if (User.Identity.IsAuthenticated)
            {
                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));
            }
            
            posts = posts
                .OrderByDescending(p => p.Votes);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
                CurrentUserId = currentUserId
            };

            return View("Index", homeViewModel);
        }

        // GET: MostUpvoted
        public async Task<IActionResult> MostDiscussed()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = await _postRepository.GetPosts();

            if (User.Identity.IsAuthenticated)
            {
                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));
            }

            posts = posts
                .OrderByDescending(p => p.Comments?.Count ?? 0);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
                CurrentUserId = currentUserId
            };

            return View("Index", homeViewModel);
        }

        // GET: NewPosts
        public async Task<IActionResult> NewPosts()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = await _postRepository.GetPosts();

            if (User.Identity.IsAuthenticated)
            {
                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));
            }

            posts = posts
                .OrderBy(p => p.Created)
                .ThenByDescending(p => p.Created.TimeOfDay);


            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
                CurrentUserId = currentUserId
            };

            return View("Index", homeViewModel);
        }

        // GET: Explore
        public async Task<IActionResult> Explore()
        {
            var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var posts = await _postRepository.GetPosts();

            Random random = new Random();

            // randomise posts
            posts = posts
                .OrderBy(p => random.Next());

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
                CurrentUserId = currentUserId
            };

            return View("Index", homeViewModel);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
