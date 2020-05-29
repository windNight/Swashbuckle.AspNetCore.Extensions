using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.Internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace System.Attributes
{
    /// <summary>
    /// 用户控制WebApi是否在SwaggerUI中显示的特性，如果打上标签的Controller或者方法 默认将不会显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenApiAttribute : Attribute, IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (ConfigItems.ShowHiddenApi) return;

            if (context.ApiDescriptions == null) return;
            foreach (var apiDescription in context.ApiDescriptions)
            {
                if (!apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>().OfType<HiddenApiAttribute>()
                    .Any()) continue;

                var key = string.Concat("/", apiDescription.RelativePath);
                if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));

                swaggerDoc.Paths.Remove(key);
            }
        }
    }
}