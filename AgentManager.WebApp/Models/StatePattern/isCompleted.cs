using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class isCompleted : State
    {
        public isCompleted(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            Console.WriteLine("Đơn hàng đã được hoàn thành");
            return false;

        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            Console.WriteLine("Đơn hàng đã được hoàn thành");
            return false;
        }

        public override bool UpdateStateOrder()
        {
            Console.WriteLine("Đơn hàng đã được hoàn thành");
            return false;
        }
    }
}
