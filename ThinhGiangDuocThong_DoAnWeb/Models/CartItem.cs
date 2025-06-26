namespace ThinhGiangDuocThong_DoAnWeb.Models
{
    public class CartItem
    {
        //khai báo các thuộc tính của giỏ hàng
        //không cần phải lưu giữ như mô tả

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
} 