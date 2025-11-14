using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Order? Order { get; set; }
}
