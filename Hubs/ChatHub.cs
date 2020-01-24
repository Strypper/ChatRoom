using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Lobby.Models;
using Lobby.Database;

namespace Lobby.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private UserManager<User> _userManager;
        private AppDbContext _ctx;

        public ChatHub(AppDbContext ctx, UserManager<User> userManager)
        {
            _userManager = userManager;
            _ctx = ctx; 
        }
        public async Task SendMessage(string message)
        {
            string userId = Context.User.Claims.First(c =>c.Type == "UserID").Value;
            var person = await _userManager.FindByIdAsync(userId);
            await Clients.All.SendAsync("ReceiveMessage", person.FirstName, message);
            Console.WriteLine("Mess Sent from " + person.FirstName + 
            " to all avaible client at " + DateTime.Now + " Which have connectionId: " + Context.ConnectionId);



            var mess = new Message
            {
                ChatId = 1005,
                Text = message,
                Name = person.UserName,
                TimeStamp = System.DateTime.Now
            };
            _ctx.Messages.Add(mess);
            await _ctx.SaveChangesAsync();
        }

        public async Task SendPrivateMessage(string message, string to)
        {
            string userId = Context.User.Claims.First(c =>c.Type == "UserID").Value;
            var person = await _userManager.FindByIdAsync(userId);
            await Clients.Caller.SendAsync("ReceiveMessage", message);
            await Clients.Client(to).SendAsync("ReceiveMessage", person.FirstName, message);
            Console.WriteLine("Mess Sent from "+ person.FirstName + " With connectionId" + Context.ConnectionId + " to " + to);
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.Claims.First(c =>c.Type == "UserID").Value;
            var person = await _userManager.FindByIdAsync(userId);
            Console.WriteLine(person.FirstName + " Just connected! " + " With ConnectionId: " + Context.ConnectionId);
            await Clients.All.SendAsync("UserConnected", person.FirstName, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string userId = Context.User.Claims.First(c =>c.Type == "UserID").Value;
            var person = await _userManager.FindByIdAsync(userId);
            Console.WriteLine(person.FirstName + " Just out!");
            await Clients.All.SendAsync("UserDisConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // private async Task<User> GetUserFromContext() 
        // {
        //     string userId = Context.User.Claims.First(c => c.Type == "UserID").Value;
        //     return await _userManager.FindByIdAsync(userId);
        // }
    }
}