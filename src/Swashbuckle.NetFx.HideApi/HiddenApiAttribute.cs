using System;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.NetFx.HideApi.Internals;
using Swashbuckle.Swagger;

namespace Swashbuckle.NetFx.HideApi
{
    public class HiddenApiAttribute : Attribute, IDocumentFilter
    {
        private static readonly string[] HiddenRoutes =
        {
            "AbpCache", "AbpServiceProxies", "ServiceProxies", "TypeScript"
        };

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            if (apiExplorer.ApiDescriptions == null) return;
            var hiddenSwagger = ConfigItems.HiddenSwagger;
            var showhiddenApi = ConfigItems.ShowHiddenApi;
            foreach (var apiDescription in apiExplorer.ApiDescriptions)
            {
                if (IsInHiddenRoutes(apiDescription.RelativePath))
                {
                    RemoveHiddenApi(swaggerDoc, apiDescription);
                    continue;
                }

                var hidden = hiddenSwagger || (!showhiddenApi && apiDescription
                    .GetControllerAndActionAttributes<HiddenApiAttribute>().OfType<HiddenApiAttribute>().Any());
                if (!hidden) continue;
                RemoveHiddenApi(swaggerDoc, apiDescription);
            }
        }

        private bool IsInHiddenRoutes(string relativePath)
        {
            var flag = false;
            foreach (var hiddenTag in HiddenRoutes)
            {
                if (relativePath.Contains(hiddenTag))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        private void RemoveHiddenApi(SwaggerDocument swaggerDoc, ApiDescription apiDescription)
        {
            var key = "/" + apiDescription.RelativePath;
            if (key.Contains("?"))
            {
                var idx = key.IndexOf("?", StringComparison.Ordinal);
                key = key.Substring(0, idx);
            }

            switch (apiDescription.HttpMethod.ToString().ToLower())
            {
                case "get":
                    swaggerDoc.paths[key].get = null;
                    break;
                case "put":
                    swaggerDoc.paths[key].put = null;
                    break;
                case "post":
                    swaggerDoc.paths[key].post = null;
                    break;
                case "delete":
                    swaggerDoc.paths[key].delete = null;
                    break;
                case "options":
                    swaggerDoc.paths[key].options = null;
                    break;
                case "head":
                    swaggerDoc.paths[key].head = null;
                    break;
                case "patch":
                    swaggerDoc.paths[key].patch = null;
                    break;
            }
        }
    }
}
