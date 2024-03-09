using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface IPromoCodeStrategy
    {
        decimal CalculateDiscount(decimal bill, FFSVoucher voucher);
    }
    public class PercentagePromoCodeStrategy : IPromoCodeStrategy
    {
        public decimal CalculateDiscount(decimal bill, FFSVoucher voucher)
        {
            decimal discountAmount = bill * (decimal)(voucher.Price / 100);
            return discountAmount;
        }
    }

    public class AmountPromoCodeStrategy : IPromoCodeStrategy
    {
        public decimal CalculateDiscount(decimal bill, FFSVoucher voucher)
        {
            return (decimal)voucher.Price;
        }
    }

    public class VoucherContextStrategy
    {
        private IPromoCodeStrategy _strategy;

        public VoucherContextStrategy(IPromoCodeStrategy strategy)
        {
            this._strategy = strategy;
        }

        public decimal CalculateDiscount(decimal bill, FFSVoucher voucher)
        {
            return _strategy.CalculateDiscount(bill, voucher);
        }
    }
}
