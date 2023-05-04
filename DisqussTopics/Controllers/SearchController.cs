using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DisqussTopics.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITopicRepository _topicRepository;

        public SearchController(IPostRepository postRepository, ICommentRepository commentRepository, ITopicRepository topicRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _topicRepository = topicRepository;
        }

        public async Task<IActionResult> Index(string searchString, string type = "posts")
        {
            ViewData["SearchString"] = searchString;

            var posts = await _postRepository.GetPosts();
            var comments = await _commentRepository.GetComments();
            var topics = await _topicRepository.GetTopics();

            if (searchString == null)
            {
                // TODO: return a view with no content 
                return NotFound();
            }

            if (!string.IsNullOrEmpty(searchString) && type == "posts")
            {
                posts = posts.Where(p => p.Title.Contains(searchString) ||
                    p.Content.Contains(searchString));

                ViewData["Type"] = type;
            }
            else if(!string.IsNullOrEmpty(searchString) && type == "comments")
            {
                comments = comments.Where(c => c.Content.Contains(searchString));
                ViewData["Type"] = type;
            }
            else if(!string.IsNullOrEmpty(searchString) && type == "topics")
            {
                topics = topics.Where(t => t.Name.ToLower().Contains(searchString));
                ViewData["Type"] = type;
            }

            var serchViewModel = new SearchViewModel()
            {
                Posts = posts,
                Comments = comments,
                Topics = topics
            };
            return View(serchViewModel);
           
        }
    }
}
