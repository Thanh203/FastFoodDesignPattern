namespace FastFoodSystem.WebApp.Models.Order
{
    public class OrderState
    {
        public static readonly string isUnconfirmed = "Chưa xác nhận";
        public static readonly string isConfirming = "Đang xác nhận";
        public static readonly string isConfirmed = "Đã xác nhận";
        public static readonly string isPaymentProcessing = "Đang thanh toán";
        public static readonly string isPaid = "Đã thanh toán";
        public static readonly string isProcessing = "Đã thực hiện";
        public static readonly string isCompleted = "Đã hoàn thành";
        public static readonly string isCancelled = "Đã hủy";
    }
}
