using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Swashbuckle.Application;
using Swashbuckle.NetFx.HideApi.Internals;

namespace Swashbuckle.NetFx.HideApi
{
    public abstract class SwaggerConfigBase
    {
        private static string _webApiVersion;
        private static string _webApiName;

        private static string WebApiVersion =>
            string.IsNullOrEmpty(_webApiVersion) ? ConfigItems.SysAppVer : _webApiVersion;

        private static string WebApiName => string.IsNullOrEmpty(_webApiName) ? ConfigItems.SysAppName : _webApiName;

        /// <summary>
        /// </summary>
        /// <param name="webApiVersion"></param>
        public static void SetWebApiVersion(string webApiVersion) => _webApiVersion =
            !string.IsNullOrEmpty(webApiVersion) ? webApiVersion : ConfigItems.SysAppVer;

        /// <summary>
        /// </summary>
        /// <param name="webApiName"></param>
        public static void SetWebApiName(string webApiName) =>
            _webApiName = !string.IsNullOrEmpty(webApiName) ? webApiName : ConfigItems.SysAppName;

        /// <summary>
        /// </summary>
        public static void RegisterBase()
        {
            RegisterBase(WebApiVersion, WebApiName);
        }

        /// <summary>
        /// </summary>
        /// <param name="webApiVersion"></param>
        /// <param name="webApiName"></param>
        public static void RegisterBase(string webApiVersion, string webApiName)
        {
            _webApiVersion = webApiVersion;
            _webApiName = webApiName;

            RegisterBase(webApiVersion, webApiName, null, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="webApiVersion"></param>
        /// <param name="webApiName"></param>
        /// <param name="configureDocsConfig"></param>
        /// <param name="configureUiConfig"></param>
        public static void RegisterBase(string webApiVersion, string webApiName,
            Action<SwaggerDocsConfig> configureDocsConfig, Action<SwaggerUiConfig> configureUiConfig)
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion(webApiVersion, webApiName);
                    GetAllXmlFilePaths().ForEach(c.IncludeXmlComments);
                    c.DocumentFilter<HiddenApiAttribute>();
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    configureDocsConfig?.Invoke(c);
                })
                .EnableSwaggerUi(c => { configureUiConfig?.Invoke(c); });
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllXmlFilePaths()
        {
            var path = string.Empty;
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

            var paths = new List<string>();
            foreach (var file in Directory.GetFiles(path).Where(m => ".xml".Equals(Path.GetExtension(m))))
            {
                paths.Add(Path.GetFullPath(file));
            }

            return paths;
        }
    }
}
