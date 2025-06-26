using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThinhGiangDuocThong_DoAnWeb.Models
{
    public class Order
    {
        //Thông tin Order
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; }
        public string Notes { get; set; }

        //Khóa ngoại user login
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        //Thông tin chi tiết giỏ hàng
        public List<OrderDetail> OrderDetails { get; set; }
    }
} 