using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using DisqussTopics.Service;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace DisqussTopics.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageService _imageService;

        public ProfileController(IUserRepository userRepository, IImageService imageService)
        {
            _userRepository = userRepository;
            _imageService = imageService;
        }

        // GET: Profile/Index/{id}
        [Route("Profile/Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var currentUser = await _userRepository
                .GetUserByIdAsync(id);

            return View(currentUser);
        }

        // GET: Profile/Edit/{id}
        [Route("Profile/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository
                .GetUserByIdAsync(id);

            var dtUserViewModel = new DTUserViewModel()
            {
                DTUsername = user.DTUsername,
                Bio = user.Bio,
            };

            return View(dtUserViewModel);
        }

        // POST: Profile/Edit/{id}
        [Route("Profile/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("DTUsername,Bio,AvatarUpload")] DTUserViewModel dTUserViewModel, string id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository
                    .GetUserByIdAsync(id);

                // Get the image result
                var imageResult = await _imageService.AddImageAsync(dTUserViewModel.AvatarUpload);
                string? imageResultURL = string.Empty;
                if (imageResult.SecureUrl != null)
                {
                    imageResultURL = imageResult.SecureUrl.ToString();

                    if (user.Avatar != null && user.Avatar.Length > 5)
                    {
                        // try to delete the old avatar
                        try
                        {
                            var fileInfo = new FileInfo(user.Avatar);
                            var publicId = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                            await _imageService.DeleteImageAsync(publicId);
                        }
                        catch (Exception)
                        {

                            ModelState.AddModelError("", "Failed to edit avatar!");
                            TempData["Error"] = "Failed to edit avatar!";
                            return View(dTUserViewModel);
                        }
                    }
                }
                else
                {
                    imageResultURL = user.Avatar;
                }

                user.DTUsername = dTUserViewModel.DTUsername;
                user.Bio = dTUserViewModel.Bio;
                user.Avatar = imageResultURL;

                _userRepository.UpdateUser(user);
                _userRepository.SaveAsync();
                return RedirectToAction("Detail", new { Id = user.Id });
            }

            return View(dTUserViewModel);
        }
    }
}
