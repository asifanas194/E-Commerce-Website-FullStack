using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace RealTimeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RealTimeEcommerceContext _context;

        public OrderController(RealTimeEcommerceContext context)
        {
            _context = context;
        }

        // DTOs
        public class CustomerInfo
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }

        public class PlaceOrderDTO
        {
            public int? UserId { get; set; }
            public decimal? TotalAmount { get; set; }
            public decimal? Discount { get; set; }
            public string PaymentMethod { get; set; }
            public CustomerInfo CustomerInfo { get; set; }
            public List<OrderDetail> OrderDetails { get; set; }
        }

        [HttpPost("place")]
        [AllowAnonymous] // Guest checkout allowed
        public IActionResult PlaceOrder([FromBody] PlaceOrderDTO data)
        {
            if (data == null || data.OrderDetails == null || data.OrderDetails.Count == 0)
                return BadRequest("No order details provided");

            if (data.CustomerInfo == null)
                return BadRequest("Customer info missing");

            try
            {
                // Main order
                var order = new Order
                {
                    UserId = data.UserId,
                    OrderDate = DateTime.Now,
                    TotalAmount = data.TotalAmount,
                    Discount = data.Discount ?? 0,
                    PaymentMethod = data.PaymentMethod,
                    CustomerName = data.CustomerInfo.Name,
                    CustomerEmail = data.CustomerInfo.Email,
                    CustomerPhone = data.CustomerInfo.Phone,
                    CustomerAddress = data.CustomerInfo.Address,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                _context.SaveChanges(); // OrderId generated

                // Order details
                foreach (var item in data.OrderDetails)
                {
                    item.OrderId = order.OrderId;
                    _context.OrderDetails.Add(item);
                }
                _context.SaveChanges();

                return Ok(new { orderId = order.OrderId, message = "Order placed successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public IActionResult GetOrders(int userId)
        {
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .ToList();
            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize] // only admin should call this in production
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Ok(orders);
        }

        [HttpPut("{id}")]
        [Authorize] // admin-only
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateDTO data)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return NotFound("Order not found");

            order.Status = data.Status;
            _context.SaveChanges();

            return Ok(new { message = "Order status updated!" });
        }

        // DTO for status update
        public class StatusUpdateDTO
        {
            public string Status { get; set; }
        }


    }
}
