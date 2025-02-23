using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroCredit.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uuid")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue,
        ErrorMessage = "Amount must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is required")]
        [Range(0.01, 100.00,
        ErrorMessage = "Interest rate must be between 0.01 and 100.00")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, ErrorMessage = "Currency must be a 3-letter ISO code")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public CreditStatus Status { get; set; }

        [Required(ErrorMessage = "Loan description is required")]
        [StringLength(500, MinimumLength = 10,
        ErrorMessage = "Loan description must be between 10 and 500 characters")]
        public string LoanDescription { get; set; }

        public Loan()
        {
            Id = Guid.NewGuid();
            Currency = string.Empty;
            LoanDescription = string.Empty;
        }

        public Loan(int userId, DateTime startDate, DateTime endDate,
        decimal amount, decimal interestRate, string currency,
        CreditStatus status, string loanDescription)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
            Amount = amount;
            InterestRate = interestRate;
            Currency = string
            .IsNullOrWhiteSpace(currency) ?
            "USD" : currency;
            Status = status;
            LoanDescription = string
            .IsNullOrWhiteSpace(loanDescription) ?
            throw new ArgumentNullException(
                nameof(loanDescription))
                : loanDescription;
        }
    }

    public enum CreditStatus
    {
        Pending,   // Approved but not yet disbursed
        Active,    // Within valid date range, waiting to be paid
        Due,       // Payment deadline passed, overdue
        Paid,      // Successfully paid
        Canceled   // Canceled credit
    }
}