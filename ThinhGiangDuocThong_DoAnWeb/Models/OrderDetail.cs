namespace ThinhGiangDuocThong_DoAnWeb.Models
{
    public class OrderDetail
    {
        //Lưu các thông tin chi tiết đơn hàng
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
} 