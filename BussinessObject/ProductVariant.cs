using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Sku { get; set; }

    public int? Stock { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product? Product { get; set; }
}
