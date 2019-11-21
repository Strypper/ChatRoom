using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lobby.Models
{
    public class Chat
    {
        public Chat()
        {
            this.Messages = new List<Message>();
            this.Users = new List<User>();
        }
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public ICollection<Message> Messages { get; set; }
            public ICollection<User> Users { get; set; }
            public ChatType Type { get; set; }
            public User Owner { get; set; }
            public string OwnerName { get; set; }
    }
}