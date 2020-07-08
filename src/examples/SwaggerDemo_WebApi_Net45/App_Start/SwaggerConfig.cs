using SwaggerDemo_WebApi_Net45;
using Swashbuckle.NetFx.HideApi;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace SwaggerDemo_WebApi_Net45
{
    public class SwaggerConfig : SwaggerConfigBase
    {
        public static void Register()
        {
            RegisterBase("v1", "SwaggerDemo_WebApi_Net45");
        }
    }

}
