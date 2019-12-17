using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lobby.Models.SignalR;
using Microsoft.AspNetCore.Identity;

namespace Lobby.Models
{
    public class User : IdentityUser
    {
        public static object Claims { get; internal set; }
        [NotMapped]
        public string Pass { get; set; }
        public string Role { get; set; }
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string CoverImageUrl { get; set; }
        [Column(TypeName = "date")]
        public DateTime DayOfBirth { get; set; }
        [Column(TypeName = "bit")]
        public bool Gender{ get; set; }
        [Required]
        [Column(TypeName = "varchar(16)")]
        public string IUID { get; set; }
        [Required]
        [Range(18, 60, ErrorMessage = "Your Age Must Older Than 18")]
        //[Column(TypeName = "tinyint")]
        public int Age { get; set; }
        public ICollection<UserRoomReference> UserChat { get; set; }

    }
}