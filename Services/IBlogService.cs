using BussinessObject;

namespace Services
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogPost>> GetAllPostsAsync();
        Task<BlogPost?> GetPostByIdAsync(int id);
        Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 3);
    }
}
