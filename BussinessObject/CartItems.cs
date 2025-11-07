using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class CartItems
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Carts? Cart { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ProductVariant? Variant { get; set; }
}
