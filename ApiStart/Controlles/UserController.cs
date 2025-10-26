using ApiStart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ApiStart.Controlles
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _db.Users.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _db.Users.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(Users user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Users user)
        {
            if (id != user.Id) return BadRequest();
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Users.FindAsync(id);
            if (item == null) return NotFound();
            _db.Users.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
