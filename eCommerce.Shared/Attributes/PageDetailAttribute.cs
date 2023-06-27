using eCommerce.Shared.Extensions;

namespace eCommerce.Shared.Attributes
{
    public sealed class PageDetailAttribute : OutputProcessorActionFilterAttribute
    {
        protected override string Process(string data)
        {
            if (data.Contains("pp-widgets"))
            {
                return data.TextRegulator();
            }
            else if (data.Contains("dp-widgets"))
            {
                return data.TextRegulator(isDP: true);
            }
            else return data;
        }
    }
}
