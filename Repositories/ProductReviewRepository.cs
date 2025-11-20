using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly SportContext _context;

        public ProductReviewRepository(SportContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                .Where(r => r.product_id == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductReview> AddReviewAsync(ProductReview review)
        {
            await _context.ProductReviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}
