using System.ComponentModel.DataAnnotations;

namespace MicroCredit.Models
{
    public class TwilioVerificationResponse
    {
        public string status { get; set; }
        public bool valid { get; set; }
    }

    public class TwilioVerificationRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+\d{10,15}$",
        ErrorMessage = "Phone number must be in E.164 format " +
        "(e.g., +1234567890) and have between 10 and 15 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
    }

    public class TwilioPhoneNumberRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+\d{10,15}$",
        ErrorMessage = "Phone number must be in E.164 format " +
        "(e.g., +1234567890) and have between 10 and 15 digits")]
        public string PhoneNumber { get; set; }
    }
}