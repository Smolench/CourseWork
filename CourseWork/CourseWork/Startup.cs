using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseWork.Startup))]
namespace CourseWork
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
