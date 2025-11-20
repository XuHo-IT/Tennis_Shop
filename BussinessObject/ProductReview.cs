using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject
{
    [Table("ProductReview")]
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("product_id")]
        public int product_id { get; set; }

        [ForeignKey("product_id")]
        public Product Product { get; set; }

        [Required]
        [Column("user_id")]
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        [MaxLength(100)]
        public string full_name { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
