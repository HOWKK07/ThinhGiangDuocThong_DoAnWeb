using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThinhGiangDuocThong_DoAnWeb.Models;
using ThinhGiangDuocThong_DoAnWeb.Repositories;

namespace ThinhGiangDuocThong_DoAnWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string searchTerm, int? categoryId)
        {
            IEnumerable<Product> products;
            
            // Lấy danh sách tất cả categories để hiển thị trong dropdown
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Tìm kiếm sản phẩm theo tên hoặc mô tả
                products = await _productRepository.GetAllAsync();
                products = products.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SearchResultsCount = products.Count();
            }
            else if (categoryId.HasValue)
            {
                // Lọc sản phẩm theo danh mục
                products = await _productRepository.GetByCategoryIdAsync(categoryId.Value);
                var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                ViewBag.SelectedCategoryName = selectedCategory?.Name;
                ViewBag.FilteredByCategory = true;
            }
            else
            {
                products = await _productRepository.GetAllAsync();
            }
            
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
