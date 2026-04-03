using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TH2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Bắt buộc phải có thẻ này để bảo mật nha
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}