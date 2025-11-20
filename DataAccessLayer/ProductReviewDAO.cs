using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductReviewDAO
    {
        private readonly DbContext _context;

        public ProductReviewDAO(DbContext context)
        {
            _context = context;
        }

        public async Task<ProductReview> AddReviewAsync(ProductReview review)
        {
            _context.Set<ProductReview>().Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Set<ProductReview>()
                .Where(r => r.product_id == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
