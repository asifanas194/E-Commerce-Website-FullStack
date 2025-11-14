using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealTimeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RealTimeEcommerceContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(RealTimeEcommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.UserEmail) ||
                    string.IsNullOrWhiteSpace(user.UserPassword))
                {
                    return BadRequest(new { message = "All fields are required!" });
                }

                if (_context.Users.Any(u => u.UserEmail == user.UserEmail))
                {
                    return BadRequest(new { message = "Email already exists!" });
                }

                user.UserId = 0; // Auto increment
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error during registration.", detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.UserEmail == login.UserEmail &&
                u.UserPassword == login.UserPassword);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials!" });

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, user.UserRole ?? "Customer")
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, Role = user.UserRole });
        }
    }
}
