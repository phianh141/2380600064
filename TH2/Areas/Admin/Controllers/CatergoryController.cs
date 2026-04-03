using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TH2.Models;
using TH2.Repositories;
using System.Threading.Tasks;

namespace TH2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Bảo mật tuyệt đối: Chỉ Admin mới được vào
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // 1. HIỂN THỊ DANH SÁCH DANH MỤC
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // 2. MỞ FORM THÊM MỚI (GET)
        public IActionResult Add()
        {
            return View();
        }

        // 3. XỬ LÝ LƯU DANH MỤC MỚI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index)); // Thêm xong quay về danh sách
            }
            return View(category);
        }

        // 4. MỞ FORM CẬP NHẬT (GET)
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục này!");
            }
            return View(category);
        }

        // 5. XỬ LÝ LƯU CẬP NHẬT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index)); // Sửa xong quay về danh sách
            }
            return View(category);
        }

        // 6. MỞ TRANG XÁC NHẬN XÓA (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục này!");
            }
            return View(category);
        }

        // 7. XỬ LÝ XÓA VĨNH VIỄN (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tùy chọn nâng cao: Bạn có thể kiểm tra xem danh mục này có đang chứa 
            // siêu xe nào không trước khi xóa để tránh lỗi khóa ngoại (Foreign Key)

            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}