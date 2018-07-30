using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OGL.Startup))]
namespace OGL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
