using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Swashbuckle.NetFx.HideApi.Internals;
using Swashbuckle.Swagger;

namespace Swashbuckle.NetFx.HideApi
{
    public class HiddenApiAttribute : Attribute, IDocumentFilter
    {
        private static string[] HiddenRoutes = new[] { "AbpCache", "AbpServiceProxies", "ServiceProxies", "TypeScript" };

        bool IsInHiddenRoutes(string relativePath)
        {
            bool flag = false;
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

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            if (apiExplorer.ApiDescriptions == null) return;
            var hiddenSwagger = ConfigItems.HiddenSwagger;
            var showhiddenApi = ConfigItems.ShowHiddenApi;
            foreach (ApiDescription apiDescription in apiExplorer.ApiDescriptions)
            {
                if (IsInHiddenRoutes(apiDescription.RelativePath))
                {
                    RemoveHiddenApi(swaggerDoc, apiDescription);
                    continue;
                }

                var hidden = hiddenSwagger || (!showhiddenApi && Enumerable.OfType<HiddenApiAttribute>(apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>()).Any());
                if (!hidden) continue;
                RemoveHiddenApi(swaggerDoc, apiDescription);
            }
        }

        private void RemoveHiddenApi(SwaggerDocument swaggerDoc, ApiDescription apiDescription)
        {
            string key = "/" + apiDescription.RelativePath;
            if (key.Contains("?"))
            {
                int idx = key.IndexOf("?", StringComparison.Ordinal);
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
                default:
                    break;
            }
        }


    }
}
