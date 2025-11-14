using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealTimeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RealTimeEcommerceContext _context;

        public CategoryController(RealTimeEcommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Categories.ToList());

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok("Category added!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, Category cat)
        {
            var existing = _context.Categories.Find(id);
            if (existing == null) return NotFound();
            existing.CategoryName = cat.CategoryName;
            _context.SaveChanges();
            return Ok("Updated successfully!");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cat = _context.Categories.Find(id);
            if (cat == null) return NotFound();
            _context.Categories.Remove(cat);
            _context.SaveChanges();
            return Ok("Deleted!");
        }
    }
}
