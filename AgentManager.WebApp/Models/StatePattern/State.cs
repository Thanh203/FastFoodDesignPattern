using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;

namespace FastFoodSystem.WebApp.Models.Order
{
    public abstract class State
    {
        protected OrderProcessor processor;

        public State(OrderProcessor processor)
        {
            this.processor = processor;
        }
        public abstract bool EditOrder(FFSOrder updatedOrder
            ,List<FFSProductOrder> products);
        public abstract bool DeleteOrder();
        public abstract bool UpdateStateOrder();
        public FFSOrder? getOrder()
        {

            if (processor.GetContext() != null)
            {
                var orders = processor.GetContext().FFSOrders;
                if (orders != null)
                    return orders.FirstOrDefault(order => order.FFSOrderId == order.FFSOrderId);
            }
            return null;
        }
        public List<FFSProductOrder>? getOrderDetails()
        {
            if (processor.GetContext() != null)
            {
                var orders = processor.GetContext().FFSProductOrders;
                if (orders != null)
                    return orders
                            .Where(item => item.FFSOrderId == processor.getOrder().FFSOrderId)
                            .OrderBy(item => item.FFSOrderId)
                            .ToList(); ;
            }
            return null;
        }

  
    }
}
