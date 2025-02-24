using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using MicroCredit.Controllers;
using MicroCredit.Services;
using MicroCredit.Data;
using Microsoft.AspNetCore.Mvc;
using MicroCredit.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<ILogger<AuthController>> _loggerMock = null!;
        private Mock<IJwtTokenService> _jwtTokenServiceMock = null!;
        private Mock<UserFingerprintService> _userFingerprintServiceMock = null!;
        private IConfiguration _configuration = null!;
        private HttpClient _httpClient = null!;
        private AuthController _controller = null!;
        private ApplicationDbContext _context = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<AuthController>>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _userFingerprintServiceMock = new Mock<UserFingerprintService>();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new ApplicationDbContext(options);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(x => x["Twilio:AccountSid"]).Returns("testAccountSid");
            configurationMock.SetupGet(x => x["Twilio:AuthToken"]).Returns("testAuthToken");
            configurationMock.SetupGet(x => x["Twilio:ServiceSid"]).Returns("testServiceSid");
            _configuration = configurationMock.Object;

            _httpClient = new HttpClient();

            _controller = new AuthController(
                _configuration,
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _userFingerprintServiceMock.Object,
                _context
            );
        }

        [TestMethod]
        public async Task Signup_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "signup",
                Phone = "+1234567890",
                Name = "TestUser"
            };

            // Act
            var result = await _controller.Signup(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Signup_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "signup",
                Phone = "invalidPhone",
                Name = "TestUser"
            };

            _controller.ModelState.AddModelError("Phone", "Invalid phone number");

            // Act
            var result = await _controller.Signup(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Signup_UserAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var existingUser = new User
            {
                Phone = "+1234567890",
                Name = "ExistingUser"
            };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var request = new SMSRequest
            {
                action = "signup",
                Phone = "+1234567890",
                Name = "TestUser"
            };

            // Act
            var result = await _controller.Signup(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_ValidRequest_ReturnsOk()
        {
            // Arrange
            var existingUser = new User
            {
                Phone = "+1234567890",
                Name = "ExistingUser"
            };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var request = new SMSRequest
            {
                action = "login",
                Phone = "+1234567890"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Login_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "login",
                Phone = "invalidPhone"
            };

            _controller.ModelState.AddModelError("Phone", "Invalid phone number");

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "login",
                Phone = "+1234567890"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}