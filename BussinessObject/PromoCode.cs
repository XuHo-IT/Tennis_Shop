using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class PromoCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public bool? IsActive { get; set; }
}
