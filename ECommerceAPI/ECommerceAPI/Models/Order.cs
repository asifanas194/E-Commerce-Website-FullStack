using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public decimal? Discount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }
}
