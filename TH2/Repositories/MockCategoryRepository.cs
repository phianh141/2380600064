using TH2.Models;

namespace TH2.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Laptop" },
                new Category { Id = 2, Name = "Desktop" }
            };
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await Task.FromResult(_categoryList);
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await Task.FromResult(_categoryList.FirstOrDefault(c => c.Id == id));
        }

        public async Task AddAsync(Category category)
        {
            category.Id = _categoryList.Max(c => c.Id) + 1;
            _categoryList.Add(category);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Category category)
        {
            var index = _categoryList.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                _categoryList[index] = category;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categoryList.Remove(category);
            }
            await Task.CompletedTask;
        }
    }
}