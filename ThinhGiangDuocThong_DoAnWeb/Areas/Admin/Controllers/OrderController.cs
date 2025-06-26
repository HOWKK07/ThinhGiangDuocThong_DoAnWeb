using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThinhGiangDuocThong_DoAnWeb.Models;

namespace ThinhGiangDuocThong_DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                //lấy luôn thông tin của user đặt đơn ==> tự map khóa ngoại và lấy thông tin
                .Include(o => o.ApplicationUser)
                .ToListAsync();
            return View(orders);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails) // Lấy thông tin các sản phẩm trong đơn hàng
                .ThenInclude(oi => oi.Product) // Lấy thông tin sản phẩm từ OrderItem
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
} 