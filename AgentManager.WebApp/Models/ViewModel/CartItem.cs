using Humanizer;
using System.Net.NetworkInformation;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class CartItem
    {
        private static CartItem instance;
        private static readonly object lockObject = new object();

        private CartItem(string fFSProductId, int quantity)
        {
            FFSProductId = fFSProductId;
            Quantity = quantity;
        }

        // Public method to get the instance of CartItem
        public static CartItem GetInstance(string fFSProductId, int quantity)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new CartItem(fFSProductId, quantity);
                    }
                }
            }
            return instance;
        }

        // Public properties
        public string FFSProductId { get; set; } // ID sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public string TenSanPham { get; set; } // Tên sản phẩm
        public string Anh { get; set; } // Đường dẫn ảnh
        public decimal Gia { get; set; } // Giá sản phẩm
        public decimal Total
        {
            get
            {
                if (Gia != null && Quantity != null) return Gia * Quantity;
                else return 0;
            }
        }
    }

}
