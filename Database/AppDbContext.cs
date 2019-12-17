using Lobby.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Lobby.Models.SignalR;

namespace Lobby.Database
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     builder.Entity<UserRoomReference>()
        //            .HasKey(ur => new { ur.UserId, ur.ChatId });
        //     builder.Entity<UserRoomReference>()
        //            .HasOne(ur => ur.User)
        //            .WithMany(u => u.UserChat)
        //            .HasForeignKey(ur => ur.UserId);
        //     builder.Entity<UserRoomReference>()
        //            .HasOne(ur => ur.Chat)
        //            .WithMany(r => r.Users)
        //            .HasForeignKey(ur => ur.ChatId);
        // }
    }
}