using System.ComponentModel.DataAnnotations;

namespace ThinhGiangDuocThong_DoAnWeb.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        //Lấy thông tin sản phẩm theo dạng khóa ngoại
        public List<Product>? Products { get; set; }
    }
}
