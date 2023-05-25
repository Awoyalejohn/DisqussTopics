using Castle.Core.Logging;
using DisqussTopics.Controllers;
using DisqussTopics.Models;
using DisqussTopics.Models.ViewModels;
using DisqussTopics.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        public async Task Index_ReturnsViewForIndex()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsHomeViewModelWithExactNumberOfPost()
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
        public async Task Index_ReturnsHomeViewModelWithPostsTheUserIsSubsribedTo()
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
        public async Task MostUpvoted_ReturnsHomeViewModelWithPostsAndIndexView()
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
        public async Task MostUpvoted_ReturnsHomeViewModelWithPostsTheUserIsSubsribedToAndIndexView()
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




    }
}
