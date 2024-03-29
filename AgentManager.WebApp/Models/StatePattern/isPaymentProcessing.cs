using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class isPaymentProcessing : State
    {
        public isPaymentProcessing(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            Console.WriteLine("Đơn hàng đang thanh toán");
                return false;
        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            Console.WriteLine("Đơn hàng đang thanh toán");
                return false;
        }

        public bool ProcessPayment()
        {
            return true;
        }

        public override bool UpdateStateOrder()
        {
            if(ProcessPayment())
            {
                processor.ChangeState(new IsPaid(processor), OrderState.isPaid);
                return true;
            }
            else
            {
                processor.ChangeState(new isConfirmed(processor), OrderState.isConfirmed);
                return false;
            }
        }
    }
}
