using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MicroCredit.Services;

namespace MicroCredit.Models
{
    public class User
    {
        [Key]
        [Column(TypeName = "uuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("FaceId")]
        public string FaceId { get; set; }

        [Required(ErrorMessage = "Phase is required.")]
        [RegularExpression(@"^[1-7]$", ErrorMessage = "Phase must be a number between 1 and 7.")]
        public int Phase { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\d{10}$",
        ErrorMessage = "Invalid phone number format. It should be a 10-digit number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30, ErrorMessage = "Name must be between 5 and 30 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration date is required.")]
        public DateTime RegDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        public string Fingerprint { get; set; }

        public User()
        {
            Phone = string.Empty;
            Name = string.Empty;
            RegDate = DateTime.UtcNow;  // Use UTC instead of Local
            Phase = 0; // Default phase value
            Fingerprint = string.Empty;
        }

        public User(string phone, string name, int phase, string fingerprint)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegDate = DateTime.UtcNow;  // Use UTC instead of Local
            Phase = phase;
            Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
        }


    }
}