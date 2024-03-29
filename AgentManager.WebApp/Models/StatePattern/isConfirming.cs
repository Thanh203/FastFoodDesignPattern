using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class isConfirming : State
    {
        public isConfirming(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            Console.WriteLine("Đơn hàng đang xác nhận");
                return false;
        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            Console.WriteLine("Đơn hàng đang xác nhận");
                return false;
        }

        public override bool UpdateStateOrder()
        {
            if(CheckingStock())
            {
                UpdateStock();
                processor.ChangeState(new isConfirmed(processor), OrderState.isConfirmed);
                return true;
            }else
            {
                processor.ChangeState(new isUnconfirmed(processor), OrderState.isUnconfirmed);
                return false;
            }
        }

        private bool CheckingStock()
        {
            //Code kiểm tra tồn kho hiện chưa được phát triển
            return true;
        }
        private void UpdateStock()
        {
            //Code cập nhật số lượng tồn kho hiện chưa được phát triển
            return;
        }
    }
}
