using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroCredit.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
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