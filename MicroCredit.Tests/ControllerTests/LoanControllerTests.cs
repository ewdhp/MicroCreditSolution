using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using MicroCredit.Controllers;
using MicroCredit.Services;
using MicroCredit.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MicroCredit.Interfaces;

namespace MicroCredit.Tests.Controllers
{
    [TestClass]
    public class LoanControllerTest
    {
        private Mock<ILoanService>? _loanServiceMock;
        private Mock<ILogger<LoanController>>? _loggerMock;
        private LoanController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _loanServiceMock = new Mock<ILoanService>();
            _loggerMock = new Mock<ILogger<LoanController>>();
            _controller = new LoanController(_loanServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetCurrentLoan_ReturnsNotFound_WhenLoanIsNull()
        {
            // Arrange
            _loanServiceMock!.Setup(service => service.GetCurrentLoanAsync()).ReturnsAsync((Loan?)null);

            // Act
            var result = await _controller!.GetCurrentLoan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetCurrentLoan_ReturnsOk_WhenLoanIsNotNull()
        {
            // Arrange
            var loan = new Loan();
            _loanServiceMock!.Setup(service => service.GetCurrentLoanAsync()).ReturnsAsync(loan);

            // Act
            var result = await _controller!.GetCurrentLoan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task CreateLoan_ReturnsBadRequest_WhenLoanAlreadyExists()
        {
            // Arrange
            var request = new CreateLoanRequest { Amount = 1000 };
            _loanServiceMock!.Setup(service => service.CreateLoanAsync(It.IsAny<decimal>())).ReturnsAsync((false, null));

            // Act
            var result = await _controller!.CreateLoan(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateLoan_ReturnsCreatedAtAction_WhenLoanIsCreated()
        {
            // Arrange
            var request = new CreateLoanRequest { Amount = 1000 };
            var loan = new Loan { Id = Guid.NewGuid() };
            _loanServiceMock!.Setup(service => service.CreateLoanAsync(It.IsAny<decimal>())).ReturnsAsync((true, loan));

            // Act
            var result = await _controller!.CreateLoan(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task GetLoan_ReturnsNotFound_WhenLoanIsNull()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            _loanServiceMock!.Setup(service => service.GetLoanByIdAsync(loanId)).ReturnsAsync((Loan?)null);

            // Act
            var result = await _controller!.GetLoan(loanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetLoan_ReturnsOk_WhenLoanIsNotNull()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var loan = new Loan();
            _loanServiceMock!.Setup(service => service.GetLoanByIdAsync(loanId)).ReturnsAsync(loan);

            // Act
            var result = await _controller!.GetLoan(loanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UpdateLoanStatus_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var request = new UpdateLoanStatusRequest { Status = 1 }; // Assuming 1 represents "Approved"
            _loanServiceMock!.Setup(service => service.UpdateLoanStatusAsync(request.Status)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller!.UpdateLoanStatus(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAllLoans_ReturnsOk_WhenLoansAreRetrieved()
        {
            // Arrange
            var loans = new List<Loan> { new Loan() };
            _loanServiceMock!.Setup(service => service.GetAllLoansAsync()).ReturnsAsync(loans);

            // Act
            var result = await _controller!.GetAllLoans();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteAllLoans_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            _loanServiceMock!.Setup(service => service.DeleteAllLoansAsync()).Returns(Task.FromResult(true));

            // Act
            var result = await _controller!.DeleteAllLoans();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}