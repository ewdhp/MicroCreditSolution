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
        public User User { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public CreditStatus Status { get; set; } = CreditStatus.Pending;

        // Parameterless constructor required by EF Core
        public Loan()
        {
            User = new User("defaultName", "defaultPhoneNumber"); // Initialize User to avoid null reference
            Currency = string.Empty; // Initialize Currency to avoid null reference
        }

        // Constructor to initialize required properties
        public Loan(User user, DateTime startDate, DateTime endDate, decimal amount, string currency)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            StartDate = startDate;
            EndDate = endDate;
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
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