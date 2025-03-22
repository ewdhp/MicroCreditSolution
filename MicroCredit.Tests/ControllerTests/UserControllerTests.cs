using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicroCredit.Controllers;
using MicroCredit.Data;
using MicroCredit.Models;
using MicroCredit.Services;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private UDbContext? _context;
        private Mock<ILogger<UserController>>? _loggerMock;
        private Mock<IJwtTokenService>? _jwtTokenServiceMock;
        private UserController? _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<UDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new UDbContext(options);
            _loggerMock = new Mock<ILogger<UserController>>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _controller = new UserController(_context, _loggerMock.Object, _jwtTokenServiceMock.Object);
        }

        [TestMethod]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenIdClaimIsMissing()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult, "Expected UnauthorizedObjectResult but got null.");
            if (unauthorizedResult != null)
            {
                var value = unauthorizedResult.Value as ErrorResponse;
                Assert.IsNotNull(value, $"Expected value to be an ErrorResponse but got {unauthorizedResult.Value?.GetType().Name}");
                if (value != null)
                {
                    Assert.AreEqual("Id claim not found in token.", value.Message);
                }
            }
        }

        [TestMethod]
        public async Task GetCurrentUser_ShouldReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new[] { new Claim("Id", userId) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult but got null.");
            if (notFoundResult != null)
            {
                var value = notFoundResult.Value as ErrorResponse;
                Assert.IsNotNull(value, $"Expected value to be an ErrorResponse but got {notFoundResult.Value?.GetType().Name}");
                if (value != null)
                {
                    Assert.AreEqual("User not found", value.Message);
                }
            }
        }

        [TestMethod]
        public async Task GetCurrentUser_ShouldReturnOk_WhenUserIsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new[] { new Claim("Id", userId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var foundUser = new User { Id = userId, Phone = "1234567890", Name = "Test User" };
            _context!.Users.Add(foundUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
            if (okResult != null)
            {
                var returnedUser = okResult.Value as UserResponse;
                Assert.IsNotNull(returnedUser, $"Expected value to be a UserResponse but got {okResult.Value?.GetType().Name}");
                if (returnedUser != null)
                {
                    Assert.AreEqual(foundUser.Id, returnedUser.Id);
                    Assert.AreEqual(foundUser.Phone, returnedUser.Phone);
                    Assert.AreEqual(foundUser.Name, returnedUser.Name);
                }
            }
        }
    }
}