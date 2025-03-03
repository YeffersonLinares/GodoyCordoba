using Microsoft.AspNetCore.Mvc;
using GodoyCordoba.Models;
using GodoyCordoba.Context;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace GodoyCordoba.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            List<User> users = _context.users.ToList();
            return Ok(users);
        }

        private int calculateScore(User user)
        {
            int score = 0;

            // Longitud del nombre del usuario
            int nameLength = user.Name.Length + user.LastName.Length;
            if (nameLength > 10)
            {
                score += 20;
            }
            else if (nameLength >= 5 && nameLength <= 10)
            {
                score += 10;
            }

            // Dominio del correo electrónico
            string emailDomain = user.Email.Split('@').Last();
            if (emailDomain == "gmail.com")
            {
                score += 40;
            }
            else if (emailDomain == "hotmail.com")
            {
                score += 20;
            }
            else
            {
                score += 10;
            }
            return score;
        }



        [HttpPost]
        public ActionResult<User> createUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            {
                int score = calculateScore(user);
                var newUser = new User
                {
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Score = score // Asignar el puntaje calculado
                };
                _context.users.Add(newUser);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update([FromBody] User user, int id)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest();
            }
            else
            {
                var existingPerson = _context.users.FirstOrDefault(p => p.Id == id);
                if (existingPerson == null)
                {
                    return NotFound();
                }
                else
                {
                    int score = calculateScore(user);
                    existingPerson.Name = user.Name;
                    existingPerson.LastName = user.LastName;
                    existingPerson.Email = user.Email;
                    // existingPerson.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    existingPerson.Score = score;
                    _context.SaveChanges();
                    return NoContent();
                }
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<User> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            else
            {
                var user = _context.users.FirstOrDefault(p => p.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.users.Remove(user);
                    _context.SaveChanges();
                    return NoContent();
                }
            }
        }
    }
}