using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId);
        Task<ProductReview> AddReviewAsync(ProductReview review);



    }
}
