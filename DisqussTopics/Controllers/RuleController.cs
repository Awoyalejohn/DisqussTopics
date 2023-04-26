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

        // GET: Topic/CreateRule/{slug}
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

        // POST: Topic/CreateRule/{slug}
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

        // GET: Topic/{slug}/EditRule/{id}
        [Route("{action}/{Slug}/{Id}")]
        public async Task<IActionResult> EditRule(int id)
        {
            var rule = await _ruleRepository
                .GetRuleByIdNoTracking(id);

            if (rule == null) NotFound();

            return View(rule);
        }

        // POST: Topic/{slug}/EditRule/{id}
        [Route("{action}/{Slug}/{Id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRule([Bind("Id,Title,Description,TopicId,Topic")] Rule ruleUpdate)
        {
            var topic = await _topicRepository
                .GetTopicByIdNoTracking(ruleUpdate.TopicId);

            ruleUpdate.Topic = topic;

            if (ModelState.IsValid)
            {
                var rule = await _ruleRepository.GetRuleById(ruleUpdate.Id);

                rule.Title = ruleUpdate.Title;
                rule.Description = ruleUpdate.Description;

                _ruleRepository.UpdateRule(rule);
                await _ruleRepository.SaveAsync();

                var slugtopic = topic.Slug;

                return RedirectToAction("Detail", "Topic", new { Slug = slugtopic });
            }

            return View(ruleUpdate);
        }

        // GET: Topic/{slug}/DeleteRule/{id}
        [Route("{action}/{Slug}/{Id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var rule = await _ruleRepository.GetRuleByIdNoTracking(id);

            if (rule == null) NotFound();

            return View(rule);
        }

        //POST: Topic/{slug}/DeleteRule/{id}
        [Route("{action}/{Slug}/{Id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteRule")]
        public async Task<IActionResult> DeleteRuleConfirmed(int id)
        {
            var rule = await _ruleRepository.GetRuleById(id);
            var slug = rule.Topic.Slug;

            if (rule == null) NotFound();

            _ruleRepository.DeleteRule(rule);
            _ruleRepository.SaveAsync();

            return RedirectToAction("Detail", "Topic", new { Slug = slug });
        }

    }
}
