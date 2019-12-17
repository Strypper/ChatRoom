using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Lobby.Models;

namespace Lobby.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private UserManager<User> _userManager;
        public ChatHub(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task SendMessage(string user, string message)
        {
            string userId = Context.User.Claims.First(c =>c.Type == "UserID").Value;
            var person = await _userManager.FindByIdAsync(userId);
            await Clients.All.SendAsync("ReceiveMessage", person, message);
            Console.WriteLine("Mess Sent from " + person.FirstName + " to all avaible client at " + DateTime.Now);
        }

        public async Task SendPrivateMessage(string user, string message, string to)
        {
            await Clients.Client(to).SendAsync("ReceiveMessage", user, message);
            Console.WriteLine("Mess Sent from " + Context.ConnectionId + " to " + to);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}