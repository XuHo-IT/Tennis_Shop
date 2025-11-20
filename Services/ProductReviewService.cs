using BussinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepo;

        public ProductReviewService(IProductReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepo.GetReviewsByProductIdAsync(productId);
            return reviews.ToList();
        }

        public Task<ProductReview> AddReviewAsync(ProductReview review)
        {
            return _reviewRepo.AddReviewAsync(review);
        }
    }
}
