using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace System.Attributes
{
    /// <summary>
    /// 控制Swagger是否显示，一般用于生产环境不想暴露在线文档的场景
    /// </summary>
    public class HiddenSwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            if (ConfigItems.HiddenSwagger) // When clear swaggerDoc.Paths
            {
                if (context.ApiDescriptions == null) return;
                try
                {
                    swaggerDoc.Info = null;
                    swaggerDoc.Paths.Clear();

                    swaggerDoc.Components.Schemas.Clear();
                }
                catch
                {
                }
            }
        }
    }


}