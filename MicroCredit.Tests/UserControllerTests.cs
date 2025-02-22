using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MicroCredit.Controllers;
using MicroCredit.Data;
using MicroCredit.Models;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private ApplicationDbContext _context;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new UserController(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetUser_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // No UserId claim
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetUser(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task GetUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task GetUser_ReturnsOk_WhenUserFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var users = new List<User>
            {
                new User { Id = userId, Phone = "1234567890" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as User;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(userId, returnValue.Id);
        }

        [TestMethod]
        public async Task CreateUser_ReturnsConflict_WhenPhoneNumberExists()
        {
            // Arrange
            var existingUser = new User { Id = Guid.NewGuid(), Phone = "1234567890" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var newUser = new User { Phone = "1234567890" };

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
        }

        [TestMethod]
        public async Task CreateUser_ReturnsCreatedAtAction_WhenUserCreated()
        {
            // Arrange
            var newUser = new User { Id = Guid.NewGuid(), Phone = "0987654321" };

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnValue = createdAtActionResult.Value as User;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(newUser.Id, returnValue.Id);
        }
    }
}