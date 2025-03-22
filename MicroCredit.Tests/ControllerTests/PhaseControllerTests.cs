using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicroCredit.Controllers;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using MicroCredit.Services;
using Moq;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class PhaseControllerTests
    {
        private Mock<IPhaseFactory>? _phaseFactoryMock;
        private Mock<ILogger<PhaseController>>? _loggerMock;
        private Mock<IServiceProvider>? _serviceProviderMock;
        private Mock<ILoanService>? _loanServiceMock;
        private PhaseController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _phaseFactoryMock = new Mock<IPhaseFactory>();
            _loggerMock = new Mock<ILogger<PhaseController>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _loanServiceMock = new Mock<ILoanService>();

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILoanService))).Returns(_loanServiceMock.Object);

            _controller = new PhaseController(
                _phaseFactoryMock.Object,
                _loggerMock.Object,
                _serviceProviderMock.Object);
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller!.NextPhase(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnNotFound_WhenPhaseIsNull()
        {
            // Arrange
            var request = new Mock<IPhaseRequest>().Object;
            _loanServiceMock!.Setup(service => service.GetCurrentLoanAsync()).ReturnsAsync(new Loan { Status = CStatus.Initial });
            _phaseFactoryMock!.Setup(factory => factory.GetPhase(CStatus.Initial)).Returns((IPhaseService)null!);

            // Act
            var result = await _controller!.NextPhase(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnNotFound_WhenResultIsNull()
        {
            // Arrange
            var request = new Mock<IPhaseRequest>().Object;
            var phaseMock = new Mock<IPhaseService>();
            _loanServiceMock!.Setup(service => service.GetCurrentLoanAsync()).ReturnsAsync(new Loan { Status = CStatus.Initial });
            _phaseFactoryMock!.Setup(factory => factory.GetPhase(CStatus.Initial)).Returns(phaseMock.Object);
            phaseMock.Setup(phase => phase.CompleteAsync(request)).ReturnsAsync((IPhaseResponse)null!);

            // Act
            var result = await _controller!.NextPhase(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task NextPhase_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new Mock<IPhaseRequest>().Object;
            var phaseMock = new Mock<IPhaseService>();
            _loanServiceMock!.Setup(service => service.GetCurrentLoanAsync()).ReturnsAsync(new Loan { Status = CStatus.Initial });
            _phaseFactoryMock!.Setup(factory => factory.GetPhase(CStatus.Initial)).Returns(phaseMock.Object);
            phaseMock.Setup(phase => phase.CompleteAsync(request)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller!.NextPhase(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult!.StatusCode);
        }
    }
}