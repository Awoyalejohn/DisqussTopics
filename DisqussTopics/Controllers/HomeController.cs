﻿using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Slugify;
using Microsoft.AspNetCore.Authorization;

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
            return View(await _postRepository.GetPostsNoTracking());
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
