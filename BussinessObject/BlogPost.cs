using System;

namespace BussinessObject;

public partial class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsPublished { get; set; }
    public int? Views { get; set; }
}
