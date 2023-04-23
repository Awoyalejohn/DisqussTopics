using DisqussTopics.Models;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DisqussTopics.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Route("Topic")]
    public class RuleController : Controller
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly ITopicRepository _topicRepository;

        public RuleController(IRuleRepository ruleRepository, ITopicRepository topicRepository)
        {
            _ruleRepository = ruleRepository;
            _topicRepository = topicRepository;
        }

        [Route("{action=CreateRule}/{slug}")] 
        public async Task<IActionResult> CreateRule(string slug)
        {
            var topic = await _topicRepository
                .GetTopicBySlugNoTrackng(slug);

            var rule = new Rule()
            {
                TopicId = topic.Id,
                Topic = topic,
            };

            return View(rule);
        }

        [HttpPost]
        [ActionName("CreateRule")]
        [Route("{action=CreateRule}/{slug}")]
        public async Task<IActionResult> SubmitRule([Bind("Id,Title,Description,TopicId")] Rule rule)
        {
            var topic = await _topicRepository.GetTopicById(rule.TopicId);
            var slug = topic.Slug;

            if (ModelState.IsValid)
            {
                if (rule == null)
                {
                    return NotFound();
                }

                _ruleRepository.InsertRule(rule);
                await _ruleRepository.SaveAsync();
                return RedirectToAction("Detail", "Topic", new { Slug = slug });
            }

            rule.Topic = topic;

            return View(rule);
        }
    }
}
