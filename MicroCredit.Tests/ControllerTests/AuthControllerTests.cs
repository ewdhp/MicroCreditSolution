using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using MicroCredit.Controllers;
using MicroCredit.Services;
using MicroCredit.Data;
using MicroCredit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<ILogger<AuthController>> _loggerMock = null!;
        private Mock<IJwtTokenService> _jwtTokenServiceMock = null!;
        private Mock<FingerprintService> _fingerprintServiceMock = null!;
        private Mock<IUserContextService> _userContextServiceMock = null!;
        private ApplicationDbContext _context = null!;
        private DefaultHttpContext _httpContext = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<AuthController>>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _fingerprintServiceMock = new Mock<FingerprintService>();
            _userContextServiceMock = new Mock<IUserContextService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            _httpContext = new DefaultHttpContext();
        }

        [TestMethod]
        public void SendSMS_InvalidPhoneNumber_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            var request = new SMSRequest { Phone = "1234567890" };

            // Act
            var result = controller.SendSMS(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void SendSMS_ValidPhoneNumber_ReturnsOk()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            var request = new SMSRequest { Phone = "+1234567890" };

            // Act
            var result = controller.SendSMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public void VerifySMS_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            controller.ModelState.AddModelError("Phone", "Required");

            var request = new SMSRequest { Phone = "+1234567890" };

            // Act
            var result = controller.VerifySMS(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void VerifySMS_InvalidCode_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            var request = new SMSRequest { Phone = "+1234567890", Code = "000000" };

            // Act
            var result = controller.VerifySMS(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void VerifySMS_ValidSignup_ReturnsOk()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            var request = new SMSRequest { Phone = "+1234567890", Code = "123456", Action = "signup" };

            // Act
            var result = controller.VerifySMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public void VerifySMS_ValidLogin_ReturnsOk()
        {
            // Arrange
            var controller = new AuthController(
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _context,
                _fingerprintServiceMock.Object,
                _userContextServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };

            var user = new User { Phone = "+1234567890", Name = "Test User", RegDate = DateTime.UtcNow };
            _context.Users.Add(user);
            _context.SaveChanges();

            var request = new SMSRequest { Phone = "+1234567890", Code = "123456", Action = "login" };

            // Act
            var result = controller.VerifySMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}