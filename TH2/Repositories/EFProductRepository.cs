using Microsoft.EntityFrameworkCore;
using TH2.Models;

namespace TH2.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // Lưu vào SQL Server
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(); // Lưu vào SQL Server
        }

        public async Task DeleteAsync(int id)
        {
            // Lấy sản phẩm lên, nhớ Include thêm bảng Images (ảnh phụ)
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                // 1. Xóa các ảnh phụ của sản phẩm này (nếu có)
                if (product.Images != null && product.Images.Any())
                {
                    _context.ProductImages.RemoveRange(product.Images);
                }

                // 2. Xóa sản phẩm chính
                _context.Products.Remove(product);

                // 3. Lưu xuống Database
                await _context.SaveChangesAsync();
            }
        }
    }
}