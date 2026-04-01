using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TH2.Models;
using TH2.Repositories;

namespace TH2.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // 1. Hiển thị danh sách sản phẩm (Chuyển sang Async)
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // 2. Hiển thị form thêm sản phẩm
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // 3. Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    // 1. Khởi tạo danh sách đối tượng ProductImage
                    product.Images = new List<ProductImage>();
                    foreach (var file in imageUrls)
                    {
                        // 2. Tạo đối tượng ProductImage mới và truyền Url vào
                        product.Images.Add(new ProductImage
                        {
                            Url = await SaveImage(file)
                        });
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction("Index");
            }

            // Nếu lỗi, load lại danh mục cho dropdown list
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // 4. Xem chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 5. Hiển thị form cập nhật
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // 6. Xử lý cập nhật
        // 6. Xử lý cập nhật
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product updatedProduct, IFormFile imageUrl, List<IFormFile> imageUrls, List<int> deleteImageIds)
        {
            if (id != updatedProduct.Id) return NotFound();
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Images");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Lấy sản phẩm cũ từ Database lên (Biến này đã được EF theo dõi)
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null) return NotFound();

                // 1. Chép từng giá trị mới từ form sang sản phẩm cũ
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.CategoryId = updatedProduct.CategoryId;

                // 2. Cập nhật ảnh đại diện mới (nếu có chọn)
                if (imageUrl != null)
                {
                    existingProduct.ImageUrl = await SaveImage(imageUrl);
                }

                // 3. Xóa các ảnh phụ được đánh dấu
                if (deleteImageIds != null && deleteImageIds.Any() && existingProduct.Images != null)
                {
                    existingProduct.Images.RemoveAll(img => deleteImageIds.Contains(img.Id));
                }

                // 4. Thêm các ảnh phụ mới (nếu có tải thêm)
                if (imageUrls != null && imageUrls.Count > 0)
                {
                    if (existingProduct.Images == null) existingProduct.Images = new List<ProductImage>();
                    foreach (var file in imageUrls)
                    {
                        existingProduct.Images.Add(new ProductImage { Url = await SaveImage(file) });
                    }
                }

                // 5. GỬI ĐÚNG BIẾN existingProduct XUỐNG DATABASE
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction("Index");
            }

            // Nếu lỗi nhập liệu, load lại dropdown danh mục
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", updatedProduct.CategoryId);
            return View(updatedProduct);
        }

        // 7. Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 8. Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        // Hàm hỗ trợ lưu file ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var savePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + fileName;
        }
    }
}