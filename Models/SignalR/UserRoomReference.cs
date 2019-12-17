using System.ComponentModel.DataAnnotations;

namespace Lobby.Models.SignalR
{
    public class UserRoomReference
    {
        [Key]
        public int Id { get; set; }
        public int UserId{get;set;}
        public User User{get;set;}
        public int ChatId{get;set;}
        public Chat Chat{get;set;}
    }
}