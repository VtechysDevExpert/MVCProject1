using eCommerce.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace eCommerce.Services
{
    public class eCommerceRoleManager : RoleManager<IdentityRole>
    {
        public eCommerceRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore)
        {
        }

        public static eCommerceRoleManager Create(IdentityFactoryOptions<eCommerceRoleManager> options, IOwinContext context)
        {
            return new eCommerceRoleManager(new RoleStore<IdentityRole>(context.Get<eCommerceContext>()));
        }
    }
}
