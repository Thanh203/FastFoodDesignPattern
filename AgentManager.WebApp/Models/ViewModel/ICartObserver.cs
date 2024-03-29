namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface ICartObserver
    {
        void Update(List<CartItem> cartItems, decimal bill);
    }
}
