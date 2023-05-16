using DisqussTopics.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Route("/Index/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var currentUser = await _userRepository
                .GetUserByIdAsync(id);

            return View(currentUser);
        }
    }
}
