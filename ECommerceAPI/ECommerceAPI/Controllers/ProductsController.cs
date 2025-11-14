using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace RealTimeEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly RealTimeEcommerceContext _context;

        public ProductsController(RealTimeEcommerceContext context)
        {
            _context = context;
        }

        // 🟢 Customer + Admin
        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Products.ToList());

        // 🟢 Get single product by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // 🟢 Admin Only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(Product p)
        {
            _context.Products.Add(p);
            _context.SaveChanges();
            return Ok("Product added successfully!");
        }

        // 🟢 Admin Only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, Product updated)
        {
            var prod = _context.Products.Find(id);
            if (prod == null) return NotFound("Not found");

            prod.ProductName = updated.ProductName;
            prod.Price = updated.Price;
            prod.Stock = updated.Stock;
            prod.Description = updated.Description;
            prod.ImageUrl = updated.ImageUrl;
            prod.CategoryId = updated.CategoryId;
            prod.Size = updated.Size;        // ✅
            prod.Color = updated.Color;      // ✅

            _context.SaveChanges();
            return Ok("Updated successfully!");
        }

        // 🟢 Admin Only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var prod = _context.Products.Find(id);
            if (prod == null) return NotFound();
            _context.Products.Remove(prod);
            _context.SaveChanges();
            return Ok("Deleted successfully!");
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";
            return Ok(new { imageUrl });
        }

        // 🟢 Get products by category
        [HttpGet("by-category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var items = _context.Products
                               .Where(p => p.CategoryId == categoryId)
                               .ToList();

            return Ok(items);
        }

    }
}
