namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class ChainOfHanderIdVoucher
    {
        readonly IHanderIdVoucher _checkSpecialCharacters = new CheckSpecialCharacters();
        readonly IHanderIdVoucher _checkSpaceCharacters = new CheckSpaceCharacters();
        readonly IHanderIdVoucher _checkNumberCharacters = new CheckNumberCharacters();

        public ChainOfHanderIdVoucher()
        {
            _checkSpaceCharacters.Successor = _checkSpecialCharacters;
            _checkSpecialCharacters.Successor = _checkNumberCharacters;
        }
        public string Handler(string id)
        {
            return _checkSpaceCharacters.RequestIdVoucher(id);
        }
    }
}
