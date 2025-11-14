using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }
}
