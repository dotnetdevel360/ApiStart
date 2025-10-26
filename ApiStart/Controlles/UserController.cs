using ApiStart.Models;
using ApiStart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiStart.Controlles
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;

        public UserController(AppDbContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

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
          // 비밀번호 해시화
     if (!string.IsNullOrEmpty(user.Password))
       {
     user.Password = HashPassword(user.Password);
     }
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Users user)
        {
            if (id != user.Id) return BadRequest();
         
       // 비밀번호 해시화
     if (!string.IsNullOrEmpty(user.Password))
         {
    user.Password = HashPassword(user.Password);
   }
         
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

   // 로그인 엔드포인트
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
     if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
       return BadRequest("Email and Password are required");
     }

      var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
   {
        return Unauthorized("Invalid email or password");
     }

            // 비밀번호 검증
      if (!VerifyPassword(request.Password, user.Password ?? ""))
            {
           return Unauthorized("Invalid email or password");
  }

            // JWT 토큰 생성
    var token = _jwtTokenService.GenerateToken(user.Id, user.Email ?? "");
       return Ok(new { token });
   }

        // 비밀번호 해시화
        private string HashPassword(string password)
        {
          using (var sha256 = SHA256.Create())
      {
   var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }
   }

        // 비밀번호 검증
   private bool VerifyPassword(string password, string hash)
        {
     var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
