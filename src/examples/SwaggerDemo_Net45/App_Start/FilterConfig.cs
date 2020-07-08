using System.Web;
using System.Web.Mvc;

namespace SwaggerDemo_Net45
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
