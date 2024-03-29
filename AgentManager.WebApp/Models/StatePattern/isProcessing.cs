using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class isProcessing : State
    {
        public isProcessing(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            Console.WriteLine("Đơn hàng đang được thực hiện");
            return false;
        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            Console.WriteLine("Đơn hàng đang được thực hiện");
            return false;
        }

        public override bool UpdateStateOrder()
        {
            processor.ChangeState(new isCompleted(processor), OrderState.isCompleted);
            return true;
        }
    }
}
