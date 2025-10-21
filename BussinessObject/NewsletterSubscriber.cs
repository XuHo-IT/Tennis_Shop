using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class NewsletterSubscriber
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? SubscribedAt { get; set; }
}
