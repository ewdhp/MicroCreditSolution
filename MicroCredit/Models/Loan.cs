using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroCredit.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign key for User

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }  // Interest rate as a percentage

        [Required]
        public string Currency { get; set; }

        [Required]
        public CreditStatus Status { get; set; } = CreditStatus.Pending;

        public string LoanPurpose { get; set; }  // Purpose of the loan

        // Parameterless constructor required by EF Core
        public Loan()
        {
            Currency = string.Empty; // Initialize Currency to avoid null reference
        }

        // Constructor to initialize required properties
        public Loan(int userId, DateTime startDate, DateTime endDate, decimal amount, decimal interestRate, string currency, string loanPurpose = null)
        {
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
            Amount = amount;
            InterestRate = interestRate;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            LoanPurpose = loanPurpose;
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