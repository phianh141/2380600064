using Microsoft.AspNetCore.Mvc;
using TH2.Extensions; // Import Helper vừa viết
using TH2.Models;
using TH2.Repositories;

namespace TH2.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ShoppingCartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Lấy giỏ hàng từ Session
        public List<CartItem> Carts
        {
            get
            {
                var data = HttpContext.Session.GetJson<List<CartItem>>("Cart");
                if (data == null)
                {
                    data = new List<CartItem>();
                }
                return data;
            }
        }

        // 1. Hiển thị giỏ hàng
        public IActionResult Index()
        {
            return View(Carts);
        }

        // 2. Thêm sản phẩm vào giỏ
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var myCart = Carts;
            var item = myCart.SingleOrDefault(p => p.ProductId == id);

            if (item == null) // Nếu chưa có thì thêm mới
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound("Không tìm thấy siêu xe này trong hệ thống.");
                }

                item = new CartItem
                {
                    ProductId = id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl // Lấy ảnh đại diện
                };
                myCart.Add(item);
            }
            else // Nếu có rồi thì tăng số lượng
            {
                item.Quantity += quantity;
            }

            // Lưu lại vào session
            HttpContext.Session.SetJson("Cart", myCart);

            // ĐÃ SỬA: Chuyển hướng thẳng sang trang hiển thị Giỏ hàng
            return RedirectToAction("Index");
        }

        // 3. Xóa sản phẩm khỏi giỏ
        public IActionResult RemoveFromCart(int id)
        {
            var myCart = Carts;
            var item = myCart.SingleOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                myCart.Remove(item);
                HttpContext.Session.SetJson("Cart", myCart);
            }
            return RedirectToAction("Index");
        }
    }
}