using ApiStart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiStart.Controlles
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 모든 Orders API는 JWT 인증 필수
    public class OrdersController : ControllerBase
    {
  private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
      {
      _db = db;
 }

        // 현재 사용자의 ID 가져오기
   private int GetCurrentUserId()
        {
      var userIdClaim = User.FindFirst("userId");
       return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetCurrentUserId();
if (userId == 0) return Unauthorized();

            var orders = await _db.Orders
          .Where(o => o.UserId == userId)
       .ToListAsync();
          return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
 var userId = GetCurrentUserId();
    if (userId == 0) return Unauthorized();

   var order = await _db.Orders.FindAsync(id);
            if (order == null || order.UserId != userId)
                return NotFound("Order not found or does not belong to you");

            return Ok(order);
        }

        [HttpPost]
 public async Task<IActionResult> Post([FromBody] Orders order)
        {
            var userId = GetCurrentUserId();
  if (userId == 0) return Unauthorized();

       // 사용자 자신의 주문만 생성 가능
    order.UserId = userId;
_db.Orders.Add(order);
       await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Orders order)
 {
 var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

 var existingOrder = await _db.Orders.FindAsync(id);
    if (existingOrder == null || existingOrder.UserId != userId)
       return NotFound("Order not found or does not belong to you");

            order.Id = id;
        order.UserId = userId;
        _db.Entry(existingOrder).State = EntityState.Detached;
     _db.Entry(order).State = EntityState.Modified;
   await _db.SaveChangesAsync();

      return NoContent();
   }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
     if (userId == 0) return Unauthorized();

            var order = await _db.Orders.FindAsync(id);
       if (order == null || order.UserId != userId)
    return NotFound("Order not found or does not belong to you");

    _db.Orders.Remove(order);
        await _db.SaveChangesAsync();

   return NoContent();
        }
    }
}
