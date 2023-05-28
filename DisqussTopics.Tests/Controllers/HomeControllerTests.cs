using DisqussTopics.Controllers;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace DisqussTopics.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _controller = new HomeController(_loggerMock.Object, _postRepositoryMock.Object);
        }

        [Fact]
        public async Task Index_ActionExecutes_ReturnsViewForIndex()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ActionExecutes_ReturnsHomeViewModelWithExactNumberOfPost()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>() {
                    new Post(),
                    new Post(),
                    new Post()
                }
                );

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.Posts.Count());

        }

        [Fact]
        public async Task Index_ActionExecutes_ReturnsHomeViewModelWithPostsTheUserIsSubsribedTo()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "2"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "3"} } } },
                });

            // Act
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewmodel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(2, viewmodel.Posts.Count());
        }

        [Fact]
        public async Task MostUpvoted_ActionExecutes_ReturnsHomeViewModelWithPostsAndIndexView()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetPosts())
            .ReturnsAsync(new List<Post>() {
                    new Post(),
                    new Post(),
                    new Post()
            }
            );

            // Act
            var result = await _controller.MostUpvoted();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);

        }

        [Fact]
        public async Task MostUpvoted_ActionExecutes_ReturnsHomeViewModelWithPostsTheUserIsSubsribedToAndIndexView()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "2"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "3"} } } },
                });

            // Act
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await _controller.MostUpvoted();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(2, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);

        }

        [Fact]
        public async Task MostDiscussed_ActionExecutes_ReturnsHomeViewModelWithPostsAndIndexView()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(),
                    new Post(),
                    new Post()
                });

            // Act
            var result = await _controller.MostDiscussed();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task MostDiscussed_ActionExecutes_ReturnsHomeViewModelWithPostsTheUserIsSubsribedToAndIndexView()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "2"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "3"} } } },
                });

            // Act
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await _controller.MostDiscussed();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(2, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task NewPosts_ActionExecutes_ReturnsHomeViewModelWithPostsAndIndexView()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(),
                    new Post(),
                    new Post()
                });

            // Act
            var result = await _controller.NewPosts();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task NewPosts_ActionExecutes_ReturnsHomeViewModelWithPostsTheUserIsSubsribedToAndIndexView()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "2"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "3"} } } },
                });

            // Act
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await _controller.NewPosts();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(2, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Explore_ActionExecutes_ReturnsHomeViewModelWithPostsAndIndexView()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(),
                    new Post(),
                    new Post()
                });

            // Act
            var result = await _controller.Explore();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.Posts.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Explore_ActionExecutes_ReturnsHomeViewModelWithPostsTheUserIsSubsribedToAndIndexView()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _postRepositoryMock.Setup(repo => repo.GetPosts())
                .ReturnsAsync(new List<Post>()
                {
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "1"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "2"} } } },
                    new Post(){ Topic = new Topic { DTUsers = new[] { new DTUser() { Id = "3"} } } },
                });

            // Act
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await _controller.Explore();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(4, viewModel.Posts.Count());
            Assert.Equal("1", viewModel.CurrentUserId);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public void Privacy_ActionExecutes_ReturnsViewForPrivacy()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ActionExecutes_ReturnsErrorViewModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.ControllerContext = controllerContext;

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

    }
}
