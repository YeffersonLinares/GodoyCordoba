using Microsoft.AspNetCore.Mvc;
using GodoyCordoba.Models;
using GodoyCordoba.Context;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity.Data;
using BCrypt.Net;

namespace GodoyCordoba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User request)
        {
            var user = _context.users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            Console.WriteLine($"Hashed Password in DB: {user.Password}"); // Debug
            Console.WriteLine($"Password Input: {request.Password}"); // Debug

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            var token = GenerateJwtToken(user);

            user.LastLogin = DateOnly.FromDateTime(DateTime.Now);
            _context.users.Update(user);
            _context.SaveChanges();

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(23), // Token válido por 2 horas
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}