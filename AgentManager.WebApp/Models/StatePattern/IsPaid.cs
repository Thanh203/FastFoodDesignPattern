using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class IsPaid : State
    {
        public IsPaid(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            Console.WriteLine("Đơn hàng đã được thanh toán");
                return false;
        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            Console.WriteLine("Đơn hàng đã được thanh toán");
            return false;
        }

        public override bool UpdateStateOrder()
        {
            processor.ChangeState(new isProcessing(processor), OrderState.isProcessing);
            return true;
        }
    }
}
