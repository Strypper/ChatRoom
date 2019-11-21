using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Lobby.Models;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Lobby.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly AppSettings _appsettings;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IOptions<AppSettings> appsettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appsettings = appsettings.Value;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]Login u)
        {
            var user = await _userManager.FindByEmailAsync(u.Email);
            if (user != null)
            {
                var result = await _signInManager
                                    .PasswordSignInAsync(user, u.Pass, false, false);
                if (result.Succeeded)
                {
                    var role = await _userManager.GetRolesAsync(user);
                    IdentityOptions _options = new IdentityOptions();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("UserID",user.Id.ToString()),
                            new Claim(_options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault())
                        }),
                        
                        Issuer = "null",
                        Audience = "null",
                        IssuedAt = DateTime.UtcNow,
                        NotBefore = DateTime.UtcNow,
                        Expires = DateTime.UtcNow.AddHours(2),
                        SigningCredentials = 
                        new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appsettings.JWT_Secret)), 
                        SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new { token });
                }
            }
            return NotFound("The user doesn't exist pls create Account");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]User u)
        {
            var user = new User
            {
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Role = u.Role,
                DayOfBirth = u.DayOfBirth,
                Gender = u.Gender,
                IUID = u.IUID,
                Age = u.Age,
                FirstName = u.FirstName,
                LastName = u.LastName
            };
            var result = await _userManager.CreateAsync(user, u.Pass);
            await _userManager.AddToRoleAsync(user, u.Role);

            if (result.Succeeded)
            {
                return Ok("Your Profile Have set-up correctly !!");
            }
            return NotFound("There is something wrong");
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetUserInfo()
        {
            string userId = User.Claims.First(c =>c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new 
            {
                user.UserName,
                user.PhoneNumber,
                user.Email,
                user.Role,
                user.DayOfBirth,
                user.Gender,
                user.IUID,
                user.ProfileImageUrl,
                user.CoverImageUrl,
                user.Age
            };
        }
    }
}