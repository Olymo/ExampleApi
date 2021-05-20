using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private static List<User> _users = new List<User>()
        {
            new User
            {
                FirstName = "Test",
                LastName = "Test",
                Username = "test",
                Password = "test"
            }
        };


        [HttpPost]
        public IActionResult Post([FromBody] LoginDto data)
        {
            var user = _users.FirstOrDefault(x => x.Username == data.Username && x.Password == data.Password);

            if(user == null)
            {
                return NotFound();
            }

            var bytes = new byte[16];
            RandomNumberGenerator.Create().GetBytes(bytes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, new Guid(bytes).ToString(), ClaimValueTypes.String, "My Issuer"),
                new Claim(JwtRegisteredClaimNames.Iss, "My Issuer", ClaimValueTypes.String, "My Issuer"),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, "My Issuer"),
                new Claim("User", JsonConvert.SerializeObject(user), ClaimValueTypes.String, "My Issuer")
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMyVerySecretKey"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: "My Issuer",
                audience: "Any",
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(1000),
                signingCredentials: credentials);

            return Ok(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
