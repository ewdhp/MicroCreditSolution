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
        public Guid Id { get; set; }

        [ForeignKey("FaceId")]
        public string FaceId { get; set; }

        [Required(ErrorMessage = "Phase is required.")]
        [NotMapped]
        public string Phase { get; set; }

        [MaxLength(256)]
        [Required(ErrorMessage = "Phase is required.")]
        public string EncryptedPhase { get; set; }

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
            RegDate = DateTime.Now;
            Phase = EncryptPhase(0); // Default phase value
            Fingerprint = string.Empty;
        }

        public User(string phone, string name, int phase, string fingerprint)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegDate = DateTime.Now;
            Phase = EncryptPhase(phase);
            Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
        }

        private string EncryptPhase(int phase)
        {
            // Implement your encryption logic here
            return Convert.ToBase64String(BitConverter.GetBytes(phase));
        }
    }
}