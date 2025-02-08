using System.Web;
using System.Web.Http;

namespace SwaggerDemo_WebApi_Net45
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //SwaggerConfig.Register();
        }
    }
}
