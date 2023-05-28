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

            var postViewModel = new PostViewModel() { Post = GetTestPost1() };

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
        public async Task Create_ModelStateValid_RedirectsToDetailAction()
        {
            // Arrange
            var user = GetTestUser();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };

            

            var tempData = _tempDataDictionary;
            tempData["Success"] = "Post created successfully!";

            _controller.TempData = tempData;
            var postViewModel = GetTestPost2();

            _topicRepositoryMock.Setup(repo => repo.GetTopicById(postViewModel.TopicId))
                .ReturnsAsync(
                new Topic()
                {
                    Id = postViewModel.TopicId,
                    Name = "Test",
                    Slug = "test",
                });

            // Act
            var result = await _controller.Create(postViewModel);
          

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

        private Post GetTestPost1()
        {
            return new Post()
            {
                Id = 1,
                Slug = "test",
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Content = "Test"
            };
        }

        private PostViewModel GetTestPost2()
        {
            return new PostViewModel()
            {
                Post = new Post()
                {
                    Title = "Test",
                    Slug = "test",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Content = "Test"
                },
                TopicId = 1,
                DTUserId = "1"
            };
        }

    }
}
