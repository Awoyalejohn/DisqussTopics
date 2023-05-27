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
            var posts = await _postRepository.GetPosts();

            posts = posts
                .OrderByDescending(p => p.Votes);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts
            };

            // Queries posts to only posts the User is subscribed to
            if (User != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));


                homeViewModel.Posts = posts;
                homeViewModel.CurrentUserId = currentUserId;
            }

            return View(homeViewModel);
        }

        // GET: MostUpvoted
        public async Task<IActionResult> MostUpvoted()
        {
            var posts = await _postRepository.GetPosts();

            posts = posts
                .OrderByDescending(p => p.Votes);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts
            };

            // Queries posts to only posts the User is subscribed to
            if (User != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));

                homeViewModel.Posts = posts;
                homeViewModel.CurrentUserId = currentUserId;
            }

            return View("Index", homeViewModel);
        }

        // GET: MostUpvoted
        public async Task<IActionResult> MostDiscussed()
        {
            var posts = await _postRepository.GetPosts();

            posts = posts
                .OrderByDescending(p => p.Comments?.Count ?? 0);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
            };

            if (User != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));

                homeViewModel.Posts = posts;
                homeViewModel.CurrentUserId = currentUserId;
            }

            return View("Index", homeViewModel);
        }

        // GET: NewPosts
        public async Task<IActionResult> NewPosts()
        {
            var posts = await _postRepository.GetPosts();

            posts = posts
                .OrderByDescending(p => p.Created)
                .ThenBy(p => p.Created.TimeOfDay);

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts
            };

            if (User != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                posts = posts
                    .Where(p => p.Topic.DTUsers
                    .Any(u => u.Id == currentUserId));

                homeViewModel.Posts = posts;
                homeViewModel.CurrentUserId = currentUserId;
            }

            return View("Index", homeViewModel);
        }

        // GET: Explore
        public async Task<IActionResult> Explore()
        {
            var posts = await _postRepository.GetPosts();

            Random random = new Random();

            // randomise posts
            posts = posts
                .OrderBy(p => random.Next());

            var homeViewModel = new HomeViewModel()
            {
                Posts = posts,
            };

            if (User != null && User.Identity.IsAuthenticated)
            {
                var currentUserId = HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

                homeViewModel.CurrentUserId = currentUserId;
            }

            return View("Index", homeViewModel);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
