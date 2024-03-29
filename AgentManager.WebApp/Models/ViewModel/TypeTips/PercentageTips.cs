namespace FastFoodSystem.WebApp.Models.ViewModel.TypeTips
{
    public class PercentTips : ITips
    {
        private static PercentTips instance;
        private readonly decimal percentage;

        private PercentTips(decimal percentage)
        {
            this.percentage = percentage;
        }

        // GetInstance to create only 1 tips 
        public static PercentTips GetInstance(decimal percentage)
        {
            if (instance == null)
            {
                instance = new PercentTips(percentage);
            }
            return instance;
        }

        public decimal CalculateTip(decimal totalAmount)
        {
            return totalAmount * (percentage / 100);
        }
    }

}
