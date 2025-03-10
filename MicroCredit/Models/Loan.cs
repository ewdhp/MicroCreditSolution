using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroCredit.Models
{
    public class Loan
    {
        [Key]
        [Column(TypeName = "uuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }  // Changed to Guid

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(50, 350, ErrorMessage = "Amount must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is required")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; } = 0.9M;

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, ErrorMessage = "Currency must be a 3-letter ISO code")]
        public string Currency { get; } = "MXN";

        [Required(ErrorMessage = "Status is required")]
        public CreditStatus Status { get; set; }

        [Required(ErrorMessage = "Loan description is required")]
        [StringLength(500, MinimumLength = 10,
        ErrorMessage = "Loan description must be between 10 and 500 characters")]
        public string LoanDescription => "Credito por 30 dias con 3% de interes diario";

        public Loan() { }

        public Loan(Guid userId, decimal amount, DateTime endDate)
        {
            if (!((endDate - DateTime.Now).Days > 30))
            {
                Id = Guid.NewGuid();
                StartDate = DateTime.Now;
                UserId = userId;
                EndDate = endDate;
                Amount = amount;
                Status = CreditStatus.Pending;
            }
        }

        public decimal DailyInterest()
        {
            return InterestRate * Amount / 30;
        }

        public decimal CurrentInterest()
        {
            var totalDays = (EndDate - StartDate).Days;
            var elapsedDays = (DateTime.Now - StartDate).Days;
            if (!(elapsedDays < 0))
                return InterestRate * Amount * totalDays / totalDays;
            return 0;
        }

        public CreditStatus GetNextPhase(CreditStatus currentStatus)
        {
            return currentStatus switch
            {
                CreditStatus.Initial => CreditStatus.Pending,
                CreditStatus.Pending => CreditStatus.Approved,
                CreditStatus.Approved => CreditStatus.Accepted,
                CreditStatus.Accepted => CreditStatus.Disbursed,
                CreditStatus.Disbursed => CreditStatus.Active,
                CreditStatus.Active => CreditStatus.Paid,
                CreditStatus.Paid => CreditStatus.Initial,
                CreditStatus.Due => CreditStatus.Canceled,
                CreditStatus.Canceled => CreditStatus.Initial,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(currentStatus), currentStatus, null)
            };
        }

        public void ProcessNextPhase()
        {
            Status = GetNextPhase(Status);
        }
    }

    public enum CreditStatus
    {
        Initial,   // Initial status
        Pending,   // Created but not yet approved
        Approved, // Approved but not yet Accepeted
        Accepted, // Accepted but not yet disbursed
        Disbursed, // Disbursed but not yet active
        Active,    // Disbursed and active
        Paid,      // Paid credit
        Due,       // Payment deadline passed   
        Canceled,   // Canceled credit
    }

    public class LoanStatusUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public CreditStatus Status { get; set; }
    }
}