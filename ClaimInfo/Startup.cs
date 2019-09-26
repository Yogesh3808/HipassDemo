using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClaimInfo.Startup))]
namespace ClaimInfo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
