using eCommerce.Data;

namespace eCommerce.Services
{
    public static class DataContextHelper
    {
        public static eCommerceContext GetNewContext()
        {
            return new eCommerceContext();
        }
    }
}
