using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroCredit.Models
{
    public enum CStatus
    {
        Pre,
        Initial,
        Create,
        Pending,
        Approved,
        Disbursed,
        Active,
        Paid,
        Due,
        Rejected,
        Unknown
    }

    public class Loan
    {
        [Key]
        [Column(TypeName = "uuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; } = GetCurrentUserId();

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(100, 300, ErrorMessage = "Amount must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is required")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; } = 0.9M;

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, ErrorMessage = "Currency must be a 3-letter")]
        public string Currency { get; } = "MXN";

        [Required(ErrorMessage = "Status is required")]
        public CStatus Status { get; set; } = CStatus.Unknown;

        [Required(ErrorMessage = "Loan description is required")]
        [StringLength(500, MinimumLength = 10,
        ErrorMessage = "Loan description must be between 10 and 500 chars")]
        public string LoanDescription => "Credito por 7 dias con 5% de interes diario";

        public Loan()
        {
            StartDate = DateTime.UtcNow;
            EndDate = StartDate.AddDays(7);
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

        private static Guid GetCurrentUserId()
        {
            // Replace this with your actual logic to retrieve the current user's ID
            // For example, if using ASP.NET Core, you might use:
            // return Guid.Parse(HttpContext.Current.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Guid.NewGuid(); // Placeholder for demonstration
        }
    }
}