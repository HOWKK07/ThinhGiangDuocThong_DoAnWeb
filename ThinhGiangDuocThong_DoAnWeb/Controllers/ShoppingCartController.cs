using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThinhGiangDuocThong_DoAnWeb.Extensions;
using ThinhGiangDuocThong_DoAnWeb.Models;
using ThinhGiangDuocThong_DoAnWeb.Repositories;

namespace ThinhGiangDuocThong_DoAnWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        //Khai báo 3 cái repository tương ứng để thực hiện quá trình thao tác csdl
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        /// <summary>
        /// View giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //Lấy thông tin từ session có tên là Cart
            //Nếu không có thông tin thì sẽ tiến hành tạo mới một giỏ hàng ShoppingCart
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        /// <summary>
        /// Lấy thông tin đầy đủ của 1 sản phẩm thông qua ID
        /// </summary>
        /// <param name="productId">Mã sản phẩm</param>
        /// <returns></returns>
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }

        /// <summary>
        /// Add sản phẩm vô giỏ hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var product = await GetProductFromDatabase(productId);
            //Tạo thông tin cartItem : sản phẩm trong giỏ hàng
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
            //Lấy về thông tin session của giỏ hàng
            //Nếu như ko có thì tạo mới giỏ hàng 
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            //Thêm sản phẩm vào giỏ hàng
            cart.AddItem(cartItem);
            //Tiến hành lưu thông tin giỏ hàng vào lại session để đi nơi khác sử dụng
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            //Điều hướng link về trang index của giỏ hàng
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Remove sản phẩm khỏi giỏ hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int productId)
        {
            //Lấy về thông tin session của giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                //Tiến hành remove sản phẩm có productID ra khỏi giỏ
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            //Điều hướng link về trang index của giỏ hàng
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Checkout giỏ hàng -- hiển thị để người dùng nhập thêm thông tin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Checkout()
        {
            return View(new Order());
        }
        /// <summary>
        /// Tiến hành lưu giỏ hàng xuống cơ sở dữ liệu
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(Order order)
        {
            //Lấy thông tin từ session cart
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống... đi về lại trang index
                return RedirectToAction("Index");
            }

            //Lấy thông tin của user đang login
            var user = await _userManager.GetUserAsync(User);
            //Gán các thông tin cần thiết vào giỏ hàng
            //id của user
            order.UserId = user.Id;
            //Ngày đặt
            order.OrderDate = DateTime.UtcNow;
            //Tổng giá
            order.TotalPrice = cart.GetTotal();
            //Danh sách sản phẩm chi tiết
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            //Thực hiện quá trình tạo đơn
            _context.Orders.Add(order);
            //Lưu thông tin đơn vào trong csdl
            await _context.SaveChangesAsync();

            //Xóa session cart trước đó.
            HttpContext.Session.Remove("Cart");

            //Đi về cái trang thông báo đặt hàng thành công
            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity < 1 ? 1 : (quantity > 99 ? 99 : quantity);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }
    }
} 