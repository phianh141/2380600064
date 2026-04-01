using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TH2.Models;
using TH2.Repositories; // Giả sử bạn đã tạo IOrderRepository

namespace TH2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Khóa cửa, chỉ Admin mới được quản lý đơn hàng
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // --- 1. INDEX: Hiển thị danh sách đơn hàng ---
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();

            // Sắp xếp đơn hàng mới nhất lên đầu (tùy chọn)
            // orders = orders.OrderByDescending(o => o.OrderDate).ToList(); 

            return View(orders);
        }

        // --- 2. DISPLAY: Xem chi tiết đơn hàng (Gồm thông tin khách và các món hàng) ---
        public async Task<IActionResult> Display(int id)
        {
            // Cần một hàm lấy đơn hàng kèm theo chi tiết (OrderDetails)
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // --- 3. UPDATE: Cập nhật trạng thái đơn hàng ---
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                // Giả sử Model Order của bạn có thuộc tính OrderStatus
                order.OrderStatus = newStatus;
                await _orderRepository.UpdateAsync(order);
            }
            return RedirectToAction(nameof(Index));
        }

        // --- 4. ADD: Thêm đơn hàng thủ công ---
        public IActionResult Add()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Order order)
        {
            ModelState.Remove("OrderDetails"); // Bỏ qua kiểm tra danh sách sản phẩm
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                await _orderRepository.AddAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // --- 5. UPDATE: Sửa thông tin giao hàng ---
        public async Task<IActionResult> Update(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Order order)
        {
            ModelState.Remove("OrderDetails");
            if (ModelState.IsValid)
            {
                // Lấy đơn hàng cũ lên để không bị mất các trường khác
                var existingOrder = await _orderRepository.GetByIdAsync(order.Id);
                if (existingOrder == null) return NotFound();

                // Chỉ cập nhật những thông tin cần thiết
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.PhoneNumber = order.PhoneNumber;
                existingOrder.ShippingAddress = order.ShippingAddress;
                existingOrder.OrderStatus = order.OrderStatus;

                await _orderRepository.UpdateAsync(existingOrder);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // --- 6. DELETE: Xóa đơn hàng ---
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}