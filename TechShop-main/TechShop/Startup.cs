using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TechShop.Startup))]
namespace TechShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
