using Humanizer;
using System.Net.NetworkInformation;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    // Interface đại diện cho các factory
    public interface ICartItemFactory
    {
        CartItem CreateCartItem(string productId, int quantity);
    }
    public class CartItemFactory : ICartItemFactory
    {
        private readonly DBHelper _dBHelper;

        public CartItemFactory(DBHelper dbHelper)
        {
            _dBHelper = dbHelper;
        }

        public CartItem CreateCartItem(string productId, int quantity)
        {
            var product = _dBHelper.GetProductByID(productId);
            if (product != null)
            {
                return new CartItem
                {
                    FFSProductId = productId,
                    tenSanPham = product.Name,
                    anh = product.Image,
                    gia = product.Price,
                    Quantity = quantity
                };
            }
            else
            {
                // Xử lý trường hợp sản phẩm không tồn tại
                return null;
            }
        }
    }

    public class CartItem
    {
        public string FFSProductId { get; set; } // ID sản phẩm
        public string tenSanPham { get; set; } // Tên sản phẩm
        public string anh { get; set; } // Đường dẫn ảnh
        public decimal gia { get; set; } // Giá sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal total
        {
            get
            {
                if (gia != null && Quantity != null) return gia * Quantity;
                else return 0;
            }
        }
    }
}
