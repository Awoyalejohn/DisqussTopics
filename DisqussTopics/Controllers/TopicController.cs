using DisqussTopics.Models;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DisqussTopics.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;

        public TopicController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        // GET: Topic/Create
        public IActionResult Create() => View();

        // POST Topic/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id, Name,Slug,Created,About,Banner,Icon")] Topic topic) 
        {
            if (ModelState.IsValid)
            {
                _topicRepository.InsertTopic(topic);
                await _topicRepository.SaveAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(topic);
        }
    }
}
