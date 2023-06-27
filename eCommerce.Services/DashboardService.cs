using System.Linq;

namespace eCommerce.Services
{
    public class DashboardService
    {
        #region Define as Singleton
        private static DashboardService _Instance;

        public static DashboardService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DashboardService();
                }

                return (_Instance);
            }
        }

        private DashboardService()
        {
        }
        #endregion

        public int GetUserCount()
        {
            var context = DataContextHelper.GetNewContext();

            return context.Users.Count();
        }
        public int GetRolesCount()
        {
            var context = DataContextHelper.GetNewContext();

            return context.Roles.Count();
        }
        public decimal GetSalesCount()
        {
            var context = DataContextHelper.GetNewContext();

            return 0; /*context.Orders.Sum(m => m.FinalAmmount);*/
        }
        public decimal GetValuesCount()
        {
            var context = DataContextHelper.GetNewContext();

            return (decimal)context.Products.Sum(m => m.Cost);
        }
    }
}
