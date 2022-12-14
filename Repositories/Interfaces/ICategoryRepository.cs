using HelloApi.Models;

namespace HelloApi.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetById(int id);
        public IQueryable<Category> GetAll();
        public Task<Category> Add(Category category);
    }
}
