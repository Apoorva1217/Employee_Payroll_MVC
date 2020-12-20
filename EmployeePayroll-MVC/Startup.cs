using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeePayroll_MVC.Startup))]
namespace EmployeePayroll_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
