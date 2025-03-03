using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MicroCredit.Controllers;
using MicroCredit.Services;
using MicroCredit.Data;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MicroCredit.Models;
using System;

namespace MicroCredit.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<ILogger<AuthController>> _loggerMock = null!;
        private Mock<IJwtTokenService> _jwtTokenServiceMock = null!;
        private Mock<UserFingerprintService> _userFingerprintServiceMock = null!;
        private Mock<ApplicationDbContext> _contextMock = null!;
        private Mock<IConfiguration> _configurationMock = null!;
        private Mock<HttpClient> _httpClientMock = null!;
        private AuthController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<AuthController>>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _userFingerprintServiceMock = new Mock<UserFingerprintService>();
            _contextMock = new Mock<ApplicationDbContext>();
            _configurationMock = new Mock<IConfiguration>();
            _httpClientMock = new Mock<HttpClient>();

            _configurationMock.SetupGet(
                x => x["Twilio:AccountSid"])
                .Returns("AC23f88289374bd1212027f88ec0bf0c27");
            _configurationMock.SetupGet(
                x => x["Twilio:AuthToken"])
                .Returns("2269e69c15c86407bbdec1a486657b0a");
            _configurationMock.SetupGet(
                x => x["Twilio:ServiceSid"])
                .Returns("VAc6245af6c94f63ff1903cb8024c918ad");

            _controller = new AuthController(
                _configurationMock.Object,
                _loggerMock.Object,
                _jwtTokenServiceMock.Object,
                _userFingerprintServiceMock.Object,
                _contextMock.Object,
                _httpClientMock.Object
            );
        }

        [TestMethod]
        public async Task SendSMS_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "signup",
                Phone = "+523321890176"
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _httpClientMock.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.SendSMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Verification SMS sent", ((dynamic)result.Value).message);
        }

        [TestMethod]
        public async Task VerifySMS_ValidSignupRequest_ReturnsOk()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "signup",
                Phone = "+523321890176",
                Code = "123456"
            };

            var twilioResponse = new MicroCredit.Controllers.TwilioVerificationResponse
            {
                status = "approved",
                valid = true
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(twilioResponse), Encoding.UTF8, "application/json")
            };

            _httpClientMock.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(responseMessage);

            _contextMock.Setup(context => context.Users.Add(It.IsAny<User>()));
            _contextMock.Setup(context => context.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.VerifySMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Signup successful", ((dynamic)result.Value).message);
        }

        [TestMethod]
        public async Task VerifySMS_ValidLoginRequest_ReturnsOk()
        {
            // Arrange
            var request = new SMSRequest
            {
                action = "login",
                Phone = "+523321890176",
                Code = "123456"
            };

            var twilioResponse = new MicroCredit.Controllers.TwilioVerificationResponse
            {
                status = "approved",
                valid = true
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(twilioResponse), Encoding.UTF8, "application/json")
            };

            _httpClientMock.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(responseMessage);

            var user = new User
            {
                Phone = request.Phone
            };

            _contextMock.Setup(context => context.Users.FirstOrDefault(It.IsAny<Func<User, bool>>())).Returns(user);

            // Act
            var result = await _controller.VerifySMS(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Login successful", ((dynamic)result.Value).message);
        }
    }
}