using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cinema_Client.Startup))]
namespace Cinema_Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
