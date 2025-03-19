using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicroCredit.Controllers;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using MicroCredit.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class PhaseControllerTests
    {
        private Mock<IPhaseFactory> _phaseFactoryMock;
        private Mock<ILogger<PhaseController>> _loggerMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<LoanService> _loanServiceMock;
        private Mock<IUserContextService> _userContextServiceMock;
        private PhaseController _controller;

        [TestInitialize]
        public void Setup()
        {
            _phaseFactoryMock = new Mock<IPhaseFactory>();
            _loggerMock = new Mock<ILogger<PhaseController>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _userContextServiceMock = new Mock<IUserContextService>();

            // Create a mock for the LoanService using the parameterless constructor
            _loanServiceMock = new Mock<LoanService>();

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(LoanService))).Returns(_loanServiceMock.Object);

            _controller = new PhaseController(_phaseFactoryMock.Object, _loggerMock.Object, _serviceProviderMock.Object);
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.NextPhase(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Request cannot be null.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnNotFound_WhenPhaseIsNull()
        {
            // Arrange
            var request = new Mock<IPhaseReq>().Object;
            _loanServiceMock.Setup(ls => ls.GetCurrentLoanAsync()).ReturnsAsync(new Loan { Status = CStatus.Initial });
            _phaseFactoryMock.Setup(pf => pf.GetPhase(It.IsAny<CStatus>())).Returns((IPhase)null);

            // Act
            var result = await _controller.NextPhase(request);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("ERROR. Phase cannot be null.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnNotFound_WhenResultIsNull()
        {
            // Arrange
            var request = new Mock<IPhaseReq>().Object;
            var phaseMock = new Mock<IPhase>();
            _loanServiceMock.Setup(ls => ls.GetCurrentLoanAsync()).ReturnsAsync(new Loan { Status = CStatus.Initial });
            _phaseFactoryMock.Setup(pf => pf.GetPhase(It.IsAny<CStatus>())).Returns(phaseMock.Object);
            phaseMock.Setup(p => p.CompleteAsync(It.IsAny<IPhaseReq>())).ReturnsAsync((IPhaseRes)null);

            // Act
            var result = await _controller.NextPhase(request);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("ERROR. Response cannot be null.", notFoundResult.Value);
        }



        [TestMethod]
        public async Task NextPhase_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new Mock<IPhaseReq>().Object;
            _loanServiceMock.Setup(ls => ls.GetCurrentLoanAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.NextPhase(request);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(500, internalServerErrorResult.StatusCode);
            Assert.AreEqual("Internal server error", internalServerErrorResult.Value);
        }
    }
}