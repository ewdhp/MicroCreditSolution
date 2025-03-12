using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MicroCredit.Services;
using MicroCredit.Models;
using MicroCredit.Data;

namespace MicroCredit.Tests
{
    public class PhaseServiceTests
    {
        private readonly Mock<ILogger<InitialService>> _initialLoggerMock;
        private readonly Mock<ILogger<ApprovalService>> _approvalLoggerMock;
        private readonly Mock<ILogger<PayService>> _payLoggerMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly ApplicationDbContext _dbContext;

        public PhaseServiceTests()
        {
            // Initialize Logger Mocks
            _initialLoggerMock = new Mock<ILogger<InitialService>>();
            _approvalLoggerMock = new Mock<ILogger<ApprovalService>>();
            _payLoggerMock = new Mock<ILogger<PayService>>();
            _userContextServiceMock = new Mock<IUserContextService>();

            // Set up In-Memory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Seed the database
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _dbContext.Loans.AddRange(new List<Loan>
        {
            new Loan { Id = Guid.NewGuid(), Amount = 5000, Status = CStatus.Pending },
            new Loan { Id = Guid.NewGuid(), Amount = 10000, Status = CStatus.Approved }
        });
            _dbContext.SaveChanges();
        }


        [Fact]
        public async Task CreateLoan_SetToPending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userContextServiceMock.Setup(x => x.GetUserId()).Returns(userId);

            var initialService = new InitialService(_initialLoggerMock.Object, _dbContext, _userContextServiceMock.Object);
            var initialRequest = new InitialRequest
            {
                Status = CStatus.Initial,
                Amount = 100,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var (success, response) = await initialService.CompleteAsync(initialRequest);

            // Assert
            Xunit.Assert.True(success);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal(CStatus.Pending, _dbContext.Loans.First().Status);
        }

        [Fact]
        public async Task ApproveLoan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userContextServiceMock.Setup(x => x.GetUserId()).Returns(userId);

            var loan = new Loan
            {
                UserId = userId,
                Status = CStatus.Pending,
                Amount = 100,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _dbContext.Loans.Add(loan);
            _dbContext.SaveChanges();

            var approvalService = new ApprovalService(_approvalLoggerMock.Object, _dbContext, _userContextServiceMock.Object);
            var pendingRequest = new PendingRequest
            {
                Status = CStatus.Pending
            };

            // Act
            var (success, response) = await approvalService.CompleteAsync(pendingRequest);

            // Assert
            Xunit.Assert.True(success);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal(CStatus.Approved, loan.Status);
        }

        [Fact]
        public async Task PayLoan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userContextServiceMock.Setup(x => x.GetUserId()).Returns(userId);

            var loan = new Loan
            {
                UserId = userId,
                Status = CStatus.Approved,
                Amount = 100,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _dbContext.Loans.Add(loan);
            _dbContext.SaveChanges();

            var payService = new PayService(_payLoggerMock.Object, _dbContext, _userContextServiceMock.Object);
            var approvalRequest = new ApprovalRequest
            {
                Status = CStatus.Approved
            };

            // Act
            var (success, response) = await payService.CompleteAsync(approvalRequest);

            // Assert
            Xunit.Assert.True(success);
            Xunit.Assert.NotNull(response);
            Xunit.Assert.Equal(CStatus.Paid, loan.Status);
        }
    }
}
