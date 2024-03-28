using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class OrderProcessor
    {
        FastFoodSystemDbContext? context;
        FFSOrder? order;
        State? state;
        int orderId;

        public OrderProcessor(int id, FastFoodSystemDbContext dbContext) 
        {
            context = dbContext;
            orderId = id; 
            order = getOrder();
            //Thiết lập trạng thái bạn đầu tùy vào trang thái đơn hang hiện tại
            initState();
        }
        public void ChangeState(State state, string newState)
        {
            this.state = state;
            order.State = newState;
            context.Update(order);
            context.SaveChanges();
        }
        public bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            return state.EditOrder(updatedOrder, products);
        }
        public bool DeleteOrder()
        {
            return state.DeleteOrder();
        }
        public bool UpdateStateOrder()
        {
            return state.UpdateStateOrder();
        }
        private void initState()
        {
            if (order.State == OrderState.isUnconfirmed)
            {
                state = new isUnconfirmed(this);
            }
            else if (order.State == OrderState.isConfirming)
            {
                state = new isConfirming(this);
            }
            else if (order.State == OrderState.isConfirmed)
            {
                state = new isConfirmed(this);
            }
            else if (order.State == OrderState.isPaymentProcessing)
            {
                state = new isPaymentProcessing(this);
            }
            else if (order.State == OrderState.isPaid)
            {
                state = new IsPaid(this);
            }
            else if (order.State == OrderState.isProcessing)
            {
                state = new isProcessing(this);
            }
            else if (order.State == OrderState.isCompleted)
            {
                state = new isCompleted(this);
            }
            else if (order.State == OrderState.isCancelled)
            {
                state = new isCancelled(this);
            }
        }
        public FastFoodSystemDbContext? GetContext()
        {
                return this.context;
        }
        public FFSOrder? getOrder() {
            if (context != null)
            {
                var orders = context.FFSOrders;
                if (orders != null)
                    return orders.FirstOrDefault(order => order.FFSOrderId == orderId);
            }
            return null;
        }
        public List<FFSProductOrder>? getOrderDetails()
        {
            return state.getOrderDetails();
        }
       
    }
}
