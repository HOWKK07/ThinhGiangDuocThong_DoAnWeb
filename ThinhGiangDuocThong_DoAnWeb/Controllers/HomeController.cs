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

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
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

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string term)
        {
            var products = await _productRepository.SearchByNameAsync(term ?? "");
            var result = products.Select(p => new {
                id = p.Id,
                name = p.Name,
                price = p.Price,
                imageUrl = (p.Images != null && p.Images.Count > 0) ? p.Images[0].Url : (p.ImageUrl ?? "/images/default.png")
            });
            return Json(result);
        }
    }
}
