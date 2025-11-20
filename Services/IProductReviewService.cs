using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProductReviewService
    {

        Task<ProductReview> AddReviewAsync(ProductReview review);

        Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId);

    }
}
