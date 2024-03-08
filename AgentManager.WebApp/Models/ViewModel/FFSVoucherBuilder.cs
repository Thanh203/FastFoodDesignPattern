using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class FFSVoucherBuilder
    {
        private string _voucherId;
        private int _num;
        private double _price;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _state;

        public FFSVoucherBuilder WithVoucherId(string voucherId)
        {
            _voucherId = voucherId;
            return this;
        }

        public FFSVoucherBuilder WithNum(int num)
        {
            _num = num;
            return this;
        }

        public FFSVoucherBuilder WithPrice(double price)
        {
            _price = price;
            return this;
        }

        public FFSVoucherBuilder WithStartDate(DateTime startDate)
        {
            _startDate = startDate;
            return this;
        }

        public FFSVoucherBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }

        public FFSVoucherBuilder WithState(string state)
        {
            _state = state;
            return this;
        }

        public FFSVoucher Build()
        {
            return new FFSVoucher
            {
                FFSVoucherId = _voucherId,
                Num = _num,
                Price = _price,
                StartDate = _startDate,
                EndDate = _endDate,
                State = _state
            };
        }
    }
}
