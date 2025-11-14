using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealTimeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly RealTimeEcommerceContext _context;

        public CartController(RealTimeEcommerceContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public IActionResult GetCart(int userId)
        {
            var cart = _context.Carts.Where(c => c.UserId == userId).ToList();
            return Ok(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
            return Ok("Item added to cart!");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCart(int id, Cart update)
        {
            var c = _context.Carts.Find(id);
            if (c == null) return NotFound();
            c.Quantity = update.Quantity;
            _context.SaveChanges();
            return Ok("Cart updated!");
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var c = _context.Carts.Find(id);
            if (c == null) return NotFound();
            _context.Carts.Remove(c);
            _context.SaveChanges();
            return Ok("Item removed!");
        }
    }
}
