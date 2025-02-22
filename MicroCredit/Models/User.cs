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
        [Column(TypeName = "uuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }  // Changed from int to Guid

        [ForeignKey("FaceId")]
        public string FaceId { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number format. It should be a 10-digit number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30, ErrorMessage = "Name must be between 5 and 30 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration date is required.")]
        public DateTime RegDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Parameterless constructor required by EF Core
        public User()
        {
            Phone = string.Empty; // Initialize Phone to avoid null reference
            Name = string.Empty; // Initialize Name to avoid null reference
            RegDate = DateTime.Now; // Initialize RegDate to avoid null reference
        }

        // Constructor to initialize required properties
        public User(string phone, string name)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegDate = DateTime.Now; // or set to a default value
        }
    }
}