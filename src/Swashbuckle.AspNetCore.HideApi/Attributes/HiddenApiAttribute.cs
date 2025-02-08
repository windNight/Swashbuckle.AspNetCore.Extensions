using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace System.Attributes
{
    /// <summary>
    ///     用户控制WebApi是否在SwaggerUI中显示的特性，如果打上标签的Controller或者方法 默认将不会显示
    ///     增加 区分 TestApi默认true   SysApi 默认false
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenApiAttribute : Attribute, IDocumentFilter
    {
        public HiddenApiAttribute()
        {
        }

        public HiddenApiAttribute(bool testApi = true, bool sysApi = false)
        {
            TestApi = testApi;
            SysApi = sysApi;
        }

        public bool TestApi { get; set; } = true;
        public bool SysApi { get; set; }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (context.ApiDescriptions == null) return;

            foreach (var apiDescription in context.ApiDescriptions)
            {
                try
                {
                    // 获取当前API的HiddenApiAttribute特性
                    var hiddenApiAttributes = apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>()
                        .OfType<HiddenApiAttribute>().ToList();

                    if (hiddenApiAttributes.IsNullOrEmpty())
                    {
                        // 如果没有HiddenApiAttribute特性，则跳过
                        continue;
                    }

                    // 检查是否需要隐藏该API
                    if (ShouldHideApi(hiddenApiAttributes))
                    {
                        var key = GetApiPathKey(apiDescription);
                        swaggerDoc.Paths.Remove(key);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private bool ShouldHideApi(List<HiddenApiAttribute> hiddenApiAttributes)
        {
            if (hiddenApiAttributes.Any(attr => attr.SysApi))
            {
                if (!ConfigItems.ShowSysApi)
                {
                    return true;
                }
            }

            if (hiddenApiAttributes.Any(attr => attr.TestApi))
            {
                if (!ConfigItems.ShowTestApi)
                {
                    return true;
                }
            }

            // 如果 ShowSysApi 为false，且存在 SysApi 为true的API，则隐藏
            //if (!ConfigItems.ShowSysApi && hiddenApiAttributes.Any(attr => attr.SysApi))
            //{
            //    return true;
            //}

            //// 如果 ShowTestApi 为false，且存在TestApi为true的API，则隐藏
            //else if (!ConfigItems.ShowTestApi && hiddenApiAttributes.Any(attr => attr.TestApi))
            //{

            //    return true;
            //}


            // 如果ShowHiddenApi为true，且所有API都显示，则不隐藏
            return !ConfigItems.ShowHiddenApi;
            //else if (ConfigItems.ShowHiddenApi)
            //{

            //    return false;
            //}


            //// 如果存在TestApi为false的API，则隐藏
            //return hiddenApiAttributes.Any(attr => !attr.TestApi);
        }

        private string GetApiPathKey(ApiDescription apiDescription)
        {
            var key = string.Concat("/", apiDescription.RelativePath);
            if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));
            return key;
        }


        public void ApplyV1(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (ConfigItems.ShowHiddenApi)
            {
                if (context.ApiDescriptions == null) return;
                foreach (var apiDescription in context.ApiDescriptions)
                {
                    // 获取当前API的HiddenApiAttribute特性
                    var hiddenApiAttributes = apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>()
                        .OfType<HiddenApiAttribute>();

                    if (hiddenApiAttributes.IsNullOrEmpty())
                    {
                        // 如果没有HiddenApiAttribute特性，则跳过
                        continue;
                    }

                    // 检查TestApi配置
                    if (hiddenApiAttributes.Any(attr => attr.TestApi))
                    {
                        // 如果TestApi为true，则根据ConfigItems.ShowTestApi决定是否隐藏
                        if (!ConfigItems.ShowTestApi)
                        {
                            // 如果ConfigItems.ShowTestApi为false，则隐藏该API
                            var key = string.Concat("/", apiDescription.RelativePath);
                            if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));

                            swaggerDoc.Paths.Remove(key);
                        }
                    }
                }


                return;
            }


            if (context.ApiDescriptions == null) return;
            foreach (var apiDescription in context.ApiDescriptions)
            {
                // 获取当前API的HiddenApiAttribute特性
                var hiddenApiAttributes = apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>()
                    .OfType<HiddenApiAttribute>();

                if (hiddenApiAttributes.IsNullOrEmpty())
                {
                    // 如果没有HiddenApiAttribute特性，则跳过
                    continue;
                }

                // 检查TestApi配置
                if (hiddenApiAttributes.Any(attr => attr.TestApi))
                {
                    // 如果TestApi为true，则根据ConfigItems.ShowTestApi决定是否隐藏
                    if (!ConfigItems.ShowTestApi)
                    {
                        // 如果ConfigItems.ShowTestApi为false，则隐藏该API
                        var key = string.Concat("/", apiDescription.RelativePath);
                        if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));

                        swaggerDoc.Paths.Remove(key);
                    }
                }
                else
                {
                    // 如果TestApi为false，则直接隐藏该API
                    var key = string.Concat("/", apiDescription.RelativePath);
                    if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));

                    swaggerDoc.Paths.Remove(key);
                }

                //var key = string.Concat("/", apiDescription.RelativePath);
                //if (key.Contains("?")) key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));

                //swaggerDoc.Paths.Remove(key);
                //swaggerDoc.Components.Schemas.Clear();
            }
        }
    }
}
