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
        private ApplicationDbContext? _context;
        private UserController? _controller;

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
            if (_context != null)
            {
                _context.Database.EnsureDeleted();
            }
            _context?.Dispose();
        }

        [TestMethod]
        public async Task GetUser_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // No UserId claim
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
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

            _controller!.ControllerContext = new ControllerContext
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

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var users = new List<User>
            {
                new User { Id = userId, Phone = "1234567890" }
            };

            if (_context != null)
            {
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as User;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(userId, returnValue.Id);
        }
        [TestMethod]
        public async Task CreateUser_ReturnsConflict_WhenPhoneNumberExists()
        {
            // Arrange
            var existingUser = new User { Id = Guid.NewGuid(), Phone = "1234567890" };
            if (_context != null)
            {
                _context.Users.Add(existingUser);
                _context.SaveChanges();
            }

            var newUser = new User { Phone = "1234567890" };

            // Act
            var result = await _controller!.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
        }
        [TestMethod]
        public async Task CreateUser_ReturnsCreatedAtAction_WhenUserCreated()
        {
            // Arrange
            var newUser = new User { Id = Guid.NewGuid(), Phone = "0987654321" };

            // Act
            var result = await _controller!.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnValue = createdAtActionResult!.Value as User;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(newUser.Id, returnValue.Id);
        }

        [TestMethod]
        public async Task UpdateUser_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // No UserId claim
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var userId = Guid.NewGuid();
            var updateUser = new User { Id = userId, Phone = "1234567890" };

            // Act
            var result = await _controller.UpdateUser(userId, updateUser);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }
        [TestMethod]
        public async Task UpdateUser_ReturnsBadRequest_WhenUserIdMismatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var updateUser = new User { Id = Guid.NewGuid(), Phone = "1234567890" };

            // Act
            var result = await _controller.UpdateUser(userId, updateUser);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task UpdateUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var updateUser = new User { Id = userId, Phone = "1234567890" };

            // Act
            var result = await _controller.UpdateUser(userId, updateUser);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task UpdateUser_ReturnsOk_WhenUserUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var existingUser = new User { Id = userId, Phone = "1234567890" };
            if (_context != null)
            {
                _context.Users.Add(existingUser);
                _context.SaveChanges();
            }

            var updateUser = new User { Id = userId, Phone = "0987654321" };

            // Act
            var result = await _controller.UpdateUser(userId, updateUser);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as User;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(updateUser.Phone, returnValue.Phone);
        }
        [TestMethod]
        public async Task UpdateUser_ThrowsConcurrencyException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var existingUser = new User { Id = userId, Phone = "1234567890", RowVersion = new byte[] { 0x00 } };
            if (_context != null)
            {
                _context.Users.Add(existingUser);
                _context.SaveChanges();
            }

            var updateUser = new User { Id = userId, Phone = "0987654321", RowVersion = new byte[] { 0x01 } };

            // Simulate concurrency exception
            if (_context != null)
            {
                _context.Entry(existingUser).OriginalValues["RowVersion"] = new byte[] { 0x00 };
            }

            // Act & Assert
            await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(() => _controller.UpdateUser(userId, updateUser));
        }


        [TestMethod]
        public async Task DeleteUser_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // No UserId claim
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.DeleteUser(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }
        [TestMethod]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task DeleteUser_ReturnsNoContent_WhenUserDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var existingUser = new User { Id = userId, Phone = "1234567890" };
            if (_context != null)
            {
                _context.Users.Add(existingUser);
                _context.SaveChanges();
            }

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}