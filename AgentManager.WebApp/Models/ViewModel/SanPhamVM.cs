using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
	public class SanPhamVM
    {
        [Display(Name = "Mã sản phẩm")]
        [Required]
        public string? maSanPham { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required]
        public string? tenSanPham { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? anh { get; set; }

        [Display(Name = "Đơn giá")]
        [Required]
        public int gia { get; set; }

        [Display(Name = "Mô tả")]
        public string? mota { get; set; }

        [Display(Name = "Loại sản phẩm")]
        public string? loaiSanPham { get; set; }
    }
}
