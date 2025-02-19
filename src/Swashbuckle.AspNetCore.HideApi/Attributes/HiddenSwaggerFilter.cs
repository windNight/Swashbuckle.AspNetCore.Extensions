using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace System.Attributes
{
    /// <summary>
    ///     控制Swagger是否显示，一般用于生产环境不想暴露在线文档的场景
    /// </summary>
    public class HiddenSwaggerFilter : IDocumentFilter //, IOperationFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (ConfigItems.HiddenSwagger) // When clear swaggerDoc.Paths
            {
                if (context.ApiDescriptions == null) return;
                try
                {
                    swaggerDoc.Components.SecuritySchemes.Clear();
                    swaggerDoc.SecurityRequirements.Clear();
                    swaggerDoc.Info = null;
                    swaggerDoc.Paths.Clear();

                    swaggerDoc.Components.Schemas.Clear();
                    swaggerDoc.Annotations?.Clear();
                }
                catch
                {
                }
            }
        }

        //public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //{
        //    if (ConfigItems.HiddenSwagger)
        //    {
        //        try
        //        {
        //            operation.Security = null; // 移除授权要求
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }


        //}
    }
}
