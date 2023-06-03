using CloudinaryDotNet.Actions;
using DisqussTopics.Controllers;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using DisqussTopics.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Net.Http;
using System.Security.Claims;

namespace DisqussTopics.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITopicRepository> _topicRepositoryMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<IVideoService> _videoServiceMock;
        private readonly PostController _controller;
        private readonly TempDataDictionary _tempDataDictionary;

        public PostControllerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _topicRepositoryMock = new Mock<ITopicRepository>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _imageServiceMock = new Mock<IImageService>();
            _videoServiceMock = new Mock<IVideoService>();
            _controller = new PostController(
                _postRepositoryMock.Object,
                _userRepositoryMock.Object,
                _topicRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _imageServiceMock.Object,
                _videoServiceMock.Object
                );
            var httpContext = new DefaultHttpContext();
            _tempDataDictionary = new TempDataDictionary(
                httpContext, Mock.Of<ITempDataProvider>()
                );
        }

        [Fact]
        public async Task Create_ActionExecutes_ReturnsViewForIndex()
        {
            // Arrange 
            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsView()
        {
            // Arrange
            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            var tempData = _tempDataDictionary;
            tempData["Error"] = "Failed to create Post!";

            _controller.TempData = tempData;

            _controller.ModelState.AddModelError("PostViewModel", "Title is required");

            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Title = null!;

            // Act
            var result = await _controller.Create(postViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<PostViewModel>(viewResult.Model);

            Assert.Equal(postViewModel.Post.Id, viewModel.Post.Id);
            Assert.Equal(postViewModel.Post.Slug, viewModel.Post.Slug);
            Assert.Equal(postViewModel.Post.Created, viewModel.Post.Created);
            Assert.Equal(postViewModel.Post.Updated, viewModel.Post.Updated);
            Assert.Equal(postViewModel.Post.Content, viewModel.Post.Content);
        }

        [Fact]
        public async Task Create_ModelStateValid_PostWithContentRedirectsToDetailAction()
        {
            // Arrange
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.InsertPost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post created successfully!";

            _controller.TempData = tempData;
            var postViewModel = GetTestPostViewModel();

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Create(postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.InsertPost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal(post.Content, postViewModel.Post.Content);
        }

        [Fact]
        public async Task Create_ModelStateValid_PostWithImageRedirectsToDetailAction()
        {
            // Arrange
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.InsertPost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post created successfully!";
            _controller.TempData = tempData;

            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Content = null;
            postViewModel.UploadImage = GetTestImage();

            _imageServiceMock.Setup(repo => repo.AddImageAsync(postViewModel.UploadImage))
                .ReturnsAsync(GetMockImageUploadResult());

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Create(postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.InsertPost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal("https://example.com/test2.png", post.Image);
        }

        [Fact]
        public async Task Create_ModelStateValid_PostWithVideoRedirectsToDetailAction()
        {
            // Arrange
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.InsertPost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post created successfully!";
            _controller.TempData = tempData;

            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Content = null;
            postViewModel.UploadVideo = GetTestVideo();

            _videoServiceMock.Setup(repo => repo.AddVideoAsync(postViewModel.UploadVideo))
                .ReturnsAsync(GetMockVideoUploadResult());

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Create(postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.InsertPost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal("https://example.com/sea-turtle.mp4", post.Video);
        }

        [Fact]
        public async Task Detail_ReturnsDetailView_UserIsNotAuthenticated()
        {
            // Arrange
            int id = 1;
            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(GetTestPost());

            // Act
            var result = await _controller.Detail(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<PostDetailViewModel>(viewResult.Model);

            Assert.Equal(1, viewModel.Post.Id);
            Assert.Equal("Test", viewModel.Post.Title);
            Assert.Equal("test", viewModel.Post.Slug);
            Assert.Equal("Test", viewModel.Post.Content);

        }

        [Fact]
        public async Task Detail_ReturnsDetailView_UserIsAuthenticated()
        {
            // Arrange
            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            var post = GetTestPost();
            post.Upvotes = new[] { new DTUser { Id = "1" } };
            post.Downvotes = Array.Empty<DTUser>();

            int id = 1;
            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(GetTestPost());

            // Act
            var result = await _controller.Detail(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<PostDetailViewModel>(viewResult.Model);

            Assert.Equal(1, viewModel.Post.Id);
            Assert.Equal("Test", viewModel.Post.Title);
            Assert.Equal("test", viewModel.Post.Slug);
            Assert.Equal("Test", viewModel.Post.Content);
        }

        [Fact]
        public async Task Edit_ActionExecutes_ReturnsViewForEdit()
        {
            // Arrange
            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            int id = 1;
            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(GetTestPost());

            // Act
            var result = await _controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<PostViewModel>(viewResult.Model);
            Assert.Equal(id, viewModel.Post.Id);
            Assert.Equal("Test", viewModel.Post.Title);
            Assert.Equal("test", viewModel.Post.Slug);
            Assert.Equal("Test", viewModel.Post.Content);
        }

        [Fact]
        public async Task Edit_InvalidModelState_ReturnsEditView()
        {
            // Arrange
            int id = 1;

            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Title = null!;

            var tempData = _tempDataDictionary;
            tempData["Error"] = "Failed to edit Post!";

            _controller.TempData = tempData;

            _controller.ModelState.AddModelError("PostViewModel", "Title is required");

            // Act
            var result = await _controller.Edit(id, postViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<PostViewModel>(viewResult.Model);
            Assert.Equal(postViewModel.Post.Id, viewModel.Post.Id);
            Assert.Equal(postViewModel.Post.Slug, viewModel.Post.Slug);
            Assert.Equal(postViewModel.Post.Created, viewModel.Post.Created);
            Assert.Equal(postViewModel.Post.Updated, viewModel.Post.Updated);
            Assert.Equal(postViewModel.Post.Content, viewModel.Post.Content);
        }

        [Fact]
        public async Task Edit_ModelStateValid_PostWithContentRedirectsToDetailAction()
        {
            // Arrange
            int id = 1;
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.UpdatePost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post edited successfully!";

            _controller.TempData = tempData;
            var postViewModel = GetTestPostViewModel();

            _postRepositoryMock.Setup(repo => repo.GetPostById(id))
                .ReturnsAsync(GetTestPost());

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Edit(id, postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.UpdatePost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal(post.Content, postViewModel.Post.Content);
        }

        [Fact]
        public async Task Edit_ModelStateValid_PostWithImageRedirectsToDetailAction()
        {
            // Arrange
            int id = 1;
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.UpdatePost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post edited successfully!";

            _controller.TempData = tempData;
            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Content = null;
            postViewModel.UploadImage = GetTestImage();

            _postRepositoryMock.Setup(repo => repo.GetPostById(id))
                .ReturnsAsync(GetTestPost());

            _imageServiceMock.Setup(repo => repo.AddImageAsync(postViewModel.UploadImage))
                .ReturnsAsync(GetMockImageUploadResult());

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Edit(id, postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.UpdatePost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal("https://example.com/test2.png", post.Image);
        }

        [Fact]
        public async Task Edit_ModelStateValid_PostWithVideoRedirectsToDetailAction()
        {
            // Arrange
            int id = 1;
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.UpdatePost(It.IsAny<Post>()))
                .Callback<Post>(p => post = p);

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post edited successfully!";

            _controller.TempData = tempData;
            var postViewModel = GetTestPostViewModel();
            postViewModel.Post.Content = null;
            postViewModel.UploadVideo = GetTestVideo();

            _postRepositoryMock.Setup(repo => repo.GetPostById(id))
                .ReturnsAsync(GetTestPost());

            _videoServiceMock.Setup(repo => repo.AddVideoAsync(postViewModel.UploadVideo))
                .ReturnsAsync(GetMockVideoUploadResult());

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(GetTestTopic());

            // Act
            var result = await _controller.Edit(id, postViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.UpdatePost(It.IsAny<Post>()), Times.Once);
            Assert.Equal(post.Title, postViewModel.Post.Title);
            Assert.Equal(post.Slug, postViewModel.Post.Slug);
            Assert.Equal("https://example.com/sea-turtle.mp4", post.Video);
        }

        [Fact]
        public async Task Delete_ActionExecutes_ReturnsNotFound()
        {
            // Arrange
            int id = 0;
            Post? post = null;
            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ActionExecutes_ReturnsViewDeleteView()
        {
            // Arrange
            int id = 1;
            Post post = GetTestPost();
            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Post>(viewResult.Model);
            Assert.Equal(post.Id, model.Id);
            Assert.Equal(post.Title, model.Title);
            Assert.Equal(post.Slug, model.Slug);
            Assert.Equal(post.Content, model.Content);

        }

        [Fact]
        public async Task DeleteConfirmed_ActionExecute_DeletesPostAndRedirects()
        {
            // Arrange
            int id = 1;
            _postRepositoryMock.Setup(repo => repo.DeletePost(It.IsAny<Post>()));

            _postRepositoryMock.Setup(repo => repo.GetPostById(id))
                .ReturnsAsync(GetTestPost());

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post deleted successfully!";

            _controller.TempData = tempData;

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _postRepositoryMock.Verify(repo => repo.DeletePost(It.IsAny<Post>()), Times.Once);
        }

        [Fact]
        public async Task Share_ActionExecutes_ReturnsSharePostPartial()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("example.com");
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            int id = 1;
            Post post = GetTestPost();

            _postRepositoryMock.Setup(repo => repo.GetPostByIdNoTracking(id))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.Share(id);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var viewModel = Assert.IsType<PostViewModel>(partialViewResult.Model);
            Assert.Equal("_SharePostPartial", partialViewResult.ViewName);
            Assert.Equal(post.Title, viewModel.Post.Title);
            Assert.Equal(post.Slug, viewModel.Post.Slug);
            Assert.Equal(post.Content, viewModel.Post.Content);
            Assert.Equal(@$"example.com/Post/Detail/{post.Topic.Name}/{post.Slug}/{id}", viewModel.URL);
        }

        [Fact]
        public async Task UpvotePost_ActionExecutes_PostGetsOneUpvote()
        {
            // Arrange
            int id = 1;
            var user = GetTestUser();
            var dtUser = GetTestDTUser();
            Post post = GetTestPost();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            _postRepositoryMock.Setup(repo => repo.GetPostById(id))
                .ReturnsAsync(post);

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("1"))
                .ReturnsAsync(dtUser);

            // Act
            var result = await _controller.UpvotePost(id);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
        }
        private ClaimsPrincipal GetTestUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));
        }

        private DTUser GetTestDTUser()
        {
            return new DTUser()
            {
                Id = "1",
                DTUsername = "test",
                PostUpvotes = new List<Post>(),
                PostDownvotes = new List<Post>()
            };
        }

        private IFormFile CreateMockImageFile(string fileName, byte[] content)
        {
            var ms = new MemoryStream(content);
            return new FormFile(ms, 0, content.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png" // or "image/jpg", "image/gif", etc.
            };
        }
        private IFormFile CreateMockVideoFile(string fileName, byte[] content)
        {
            var ms = new MemoryStream(content);
            return new FormFile(ms, 0, content.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4" // or "image/jpg", "image/gif", etc.
            };
        }

        private IFormFile GetTestImage()
        {
            var imageContent = File.ReadAllBytes("C:/Users/John/Documents/GitHub/DisqussTopics/DisqussTopics/wwwroot/images/test2.png"); // or test.jpg, test.gif, etc.
            var mockImage = CreateMockImageFile("C:/Users/John/Documents/GitHub/DisqussTopics/DisqussTopics/wwwroot/images/test2.png", imageContent);
            return mockImage;
        }

        private IFormFile GetTestVideo()
        {
            var videoContent = File.ReadAllBytes("C:/Users/John/Documents/GitHub/DisqussTopics/DisqussTopics/wwwroot/videos/sea-turtle.mp4"); // or test.jpg, test.gif, etc.
            var mockvideo = CreateMockVideoFile("C:/Users/John/Documents/GitHub/DisqussTopics/DisqussTopics/wwwroot/videos/sea-turtle.mp4", videoContent);
            return mockvideo;
        }
        private ImageUploadResult GetMockImageUploadResult()
        {
            return new ImageUploadResult()
            {
                PublicId = "mock_public_id",
                Version = "1234567890",
                Format = "png",
                Width = 800,
                Height = 600,
                SecureUrl = new Uri("https://example.com/test2.png"),
                OriginalFilename = "test2.png"
            };
        }
        private VideoUploadResult GetMockVideoUploadResult()
        {
            return new VideoUploadResult()
            {
                PublicId = "mock_public_id",
                Version = "1234567890",
                Format = "mp4",
                Width = 1920,
                Height = 1080,
                SecureUrl = new Uri("https://example.com/sea-turtle.mp4"),
                OriginalFilename = "sea-turtle.mp4",
                Duration = 60 // Duration in seconds
            };
        }

        private PostViewModel GetTestPostViewModel()
        {
            return new PostViewModel()
            {
                Post = new Post()
                {
                    Title = "Test",
                    Slug = "test",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Content = "Test",
                },
                TopicId = 1,
                DTUserId = "1"
            };
        }

        private Post GetTestPost()
        {
            return new Post()
            {
                Id = 1,
                Title = "Test",
                Slug = "test",
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Content = "Test",
                TopicId = 1,
                Topic = new Topic() { Name = "Test" },
                Upvotes = new List<DTUser>(),
                Downvotes = new List<DTUser>()
            };
        }

        private Topic GetTestTopic()
        {
            return new Topic()
            {
                Id = 1,
                Name = "Test",
                Slug = "test",
            };
        }

    }
}
