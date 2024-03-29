using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics;

namespace FastFoodSystem.WebApp.Models.Order
{
    public class isUnconfirmed : State
    {
        public isUnconfirmed(OrderProcessor processor) : base(processor)
        {
        }

        public override bool DeleteOrder()
        {
            FastFoodSystemDbContext? context = processor.GetContext();
            FFSOrder order = processor.getOrder();

            if (order != null)
            {
                List<FFSProductOrder> orderDetail = getOrderDetails();

                foreach (var product in orderDetail)
                {
                    context.FFSProductOrders.Remove(product);
                }

                context.SaveChanges();
                context.FFSOrders.Remove(order);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public override bool EditOrder(FFSOrder updatedOrder, List<FFSProductOrder> products)
        {
            FastFoodSystemDbContext? context = processor.GetContext();
            FFSOrder order = processor.getOrder();

            if (context != null)
            {
                List<FFSProductOrder> _products = getOrderDetails();
                double updatedCash = 0;
                foreach (var product in products)
                {
                    var existingProduct = _products.FirstOrDefault(item => item.FFSProductId == product.FFSProductId);
                    int pricePr = context.FFSProducts.FirstOrDefault(item => item.FFSProductId == existingProduct.FFSProductId).Price;
                    if (existingProduct != null)
                    {
                        existingProduct.Quantity = product.Quantity;
                        context.Entry(existingProduct).State = EntityState.Modified;
                        updatedCash += product.Quantity * pricePr;
                    }
                }
                order.Cash = updatedCash;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public override bool UpdateStateOrder()
        {
            processor.ChangeState(new isConfirming(processor), OrderState.isConfirming);
            if (processor.UpdateStateOrder())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
