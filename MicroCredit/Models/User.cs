using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MicroCredit.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number format. It should be a 10-digit number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration date is required.")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "At least one image is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public List<string> Images { get; set; }

        // Parameterless constructor required by EF Core
        public User()
        {
            PhoneNumber = string.Empty; // Initialize PhoneNumber to avoid null reference
            Name = string.Empty; // Initialize Name to avoid null reference
            RegistrationDate = DateTime.Now; // Initialize RegistrationDate to avoid null reference
            Images = new List<string>(); // Initialize Images to avoid null reference
        }

        // Constructor to initialize required properties
        public User(string phoneNumber, string name)
        {
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegistrationDate = DateTime.Now; // or set to a default value
            Images = new List<string>();
        }
    }
}
