using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ImageId { get; set; }

    public bool? IsMain { get; set; }

    public bool? IsPrimary { get; set; }

    public virtual Product? Product { get; set; }
}
