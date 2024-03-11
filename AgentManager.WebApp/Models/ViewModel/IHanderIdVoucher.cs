using FastFoodSystem.WebApp.Migrations;
using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface IHanderIdVoucher
    {
        IHanderIdVoucher Successor {  get; set; }
        string RequestIdVoucher(string id);
    }
    
    public class CheckSpecialCharacters : IHanderIdVoucher
    {
        public IHanderIdVoucher Successor { get; set; }
        public string RequestIdVoucher(string id) 
        {
            if (!char.IsLetterOrDigit(id[0]) && !char.IsWhiteSpace(id[0]))
            {
                return "Kí tự đầu không được là kí tự đặc biệt";
            }
            return Successor.RequestIdVoucher(id);
        }
    }

    public class CheckNumberCharacters : IHanderIdVoucher
    {
        public IHanderIdVoucher Successor { get; set; }
        public string RequestIdVoucher(string id)
        {
            if (char.IsDigit(id[0]))
            {
                return "Kí tự đầu không được là kí tự số";
            }
            return null;
        }
    }

    public class CheckSpaceCharacters : IHanderIdVoucher
    {
        public IHanderIdVoucher Successor { get; set; }
        public string RequestIdVoucher(string id)
        {
            if (id.StartsWith(" "))
            {
                return "Không được có khoảng trống ở đầu";
            }
            return Successor.RequestIdVoucher(id);
        }
    }
}
