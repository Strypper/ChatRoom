using Lobby.Database;
using Lobby.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Lobby.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize(Roles="Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class LobbyController : ControllerBase
    {
        private AppDbContext _ctx;
        private UserManager<User> _userManager;
        public LobbyController(AppDbContext ctx, UserManager<User> userManager) {_ctx = ctx; _userManager = userManager;}
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<Chat> AllRooms()
        {
            var rooms = _ctx.Chats;
            return rooms.ToList();
        }
        [HttpGet("{id}")]
        public IActionResult Room(int id)
        {
            var chat = _ctx.Chats
                        .Include(x => x.Messages)
                        .FirstOrDefault(x => x.Id == id);
            return Ok(chat);
        }
        [HttpPost]
        public async Task<IActionResult> JoinRoom(int id)
        {
            string userId = User.Claims.First(c =>c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            
            //_ctx.Users.Add();

            await _ctx.SaveChangesAsync();
            return Ok();
        }
        // [HttpPost]
        // public async Task<IActionResult> CreateMessage([FromBody]Message sendmess)
        // {
        //     string userId = User.Claims.First(c =>c.Type == "UserID").Value;
        //     var user = await _userManager.FindByIdAsync(userId);
        //     var mess = new Message
        //     {
        //         ChatId = sendmess.ChatId,
        //         Text = sendmess.Text,
        //         Name = user.UserName,
        //         TimeStamp = System.DateTime.Now
        //     };
        //     _ctx.Messages.Add(mess);
        //     await _ctx.SaveChangesAsync();
        //     return Ok(mess);
        // }
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody]Chat chat)
        {
            string userId = User.Claims.First(c =>c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            _ctx.Chats.Add(new Chat{
                Name = chat.Name,
                Type = chat.Type,
                Owner = user,
                OwnerName = user.UserName
            });
            await _ctx.SaveChangesAsync();
            return Ok();
        }
    }
}