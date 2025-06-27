using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThinhGiangDuocThong_DoAnWeb.Models;
using ThinhGiangDuocThong_DoAnWeb.Repositories;

namespace ThinhGiangDuocThong_DoAnWeb.Controllers
{
    [Authorize]
    public class OrderHistoryController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderHistoryController(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var orders = await _orderRepository.GetByUserIdAsync(currentUser.Id);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra xem đơn hàng có thuộc về user hiện tại không
            if (order.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(order);
        }
    }
} 