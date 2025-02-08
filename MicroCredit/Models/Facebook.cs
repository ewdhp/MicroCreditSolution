#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace MicroCredit.Models
{
    public class Facebook
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public int FriendCount { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public Facebook(string id, string userId, string name, string pictureUrl, int friendCount)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PictureUrl = pictureUrl ?? throw new ArgumentNullException(nameof(pictureUrl));
            FriendCount = friendCount;
            RegistrationDate = DateTime.Now;
        }
    }
}
