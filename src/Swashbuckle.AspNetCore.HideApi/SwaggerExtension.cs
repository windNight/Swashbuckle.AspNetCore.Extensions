using System.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.Abstractions;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.HideApi;
using Swashbuckle.AspNetCore.HideApi.@internal;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Swashbuckle.AspNetCore.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        ///     Init Config
        ///     AddSwaggerGen
        ///     Add HiddenApiAttribute And HiddenSwaggerFilter
        /// </summary>
        /// <param name="services"></param>
        /// <param name="title"></param>
        /// <param name="configuration"></param>
        /// <param name="swaggerConfig"></param>
        /// <param name="apiVersion"></param>
        /// <param name="swaggerGenOptionsAction"></param>
        /// <param name="paramUpperCamelCase"></param>
        /// <remarks>
        ///     add AddSwaggerGen with DocumentFilter
        ///     1、 <see cref="System.Attributes.HiddenApiAttribute" />
        ///     2、 <see cref="System.Attributes.HiddenSwaggerFilter" />
        ///     add all files like *.xml in AppContext.BaseDirectory
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, string title,
            IConfiguration configuration, ISwaggerConfig swaggerConfig = null,
            string apiVersion = "v1", Action<SwaggerGenOptions> swaggerGenOptionsAction = null,
            bool paramUpperCamelCase = true, Dictionary<string, string> signKeyDict = null,
            Dictionary<string, string> resDict = null, bool useDefaultRes = false)
        {
            if (swaggerConfig != null)
            {
                ConfigItems.InitSwaggerConfig(swaggerConfig);
            }

            ConfigItems.InitConfiguration(configuration);

            services.AddSwaggerGen(c =>
            {
                //c.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    if (ConfigItems.ShowHiddenApi) return false;

                //    // 根据apiDesc来决定是否包含某个API到Swagger文档
                //    return true; // 或者根据条件返回false
                //});


                if (!paramUpperCamelCase)
                {
                    c.DescribeAllParametersInCamelCase();
                }


                c.SchemaFilter<PascalCaseSchemaFilter>();
                //c.SchemaFilter<HiddenSchemasResolver>();

                c.DocumentFilter<HiddenApiAttribute>();
                c.DocumentFilter<HiddenSwaggerFilter>();
                //c.OperationFilter<HiddenSwaggerFilter>();


                c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = title, Version = apiVersion });
                //if (XmlHelper.Instance.DocumentFiles.IsNullOrEmpty())
                //{
                var path = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(path))
                {
                    if (".xml".Equals(Path.GetExtension(file)))
                    {
                        c.IncludeXmlComments(Path.GetFullPath(file));
                    }
                }
                //}
                //else
                //{
                //    XmlHelper.Instance.DocumentFiles.ForEach(m => c.IncludeXmlComments(m));
                //}

                swaggerGenOptionsAction?.Invoke(c);

                ProcessSecurityMode(c, configuration, signKeyDict);
                //ProcessResponseMode(c, configuration, resDict);
            });

            return services;
        }

        private static void ProcessResponseMode(SwaggerGenOptions c, IConfiguration configuration,
            Dictionary<string, string> resDict = null, bool useDefaultRes = false)
        {
            if (resDict.IsNullOrEmpty())
            {
                var sectionKey = nameof(SwaggerConfigs);
                var config = configuration.GetSection(sectionKey).Get<SwaggerConfigs>();
                resDict = config.ResConfigs;
                if (resDict.IsNullOrEmpty() && useDefaultRes)
                {
                    resDict = new Dictionary<string, string>
                    {
                        { "0", "OK" },
                        { "100400", "BadRequest" },
                        { "100401", "Unauthorized" },
                        { "100404", "NOT FOUND" },
                        { "100500", "SystemError" },
                    };
                }
            }

            if (resDict.IsNullOrEmpty())
            {
                return;
            }

            c.OperationFilter<CustomResponseOperationFilter>(resDict);


        }

        private static void ProcessSecurityMode(SwaggerGenOptions c, IConfiguration configuration,
            Dictionary<string, string> signKeyDict = null)
        {
            if (signKeyDict.IsNullOrEmpty())
            {
                var sectionKey = nameof(SwaggerConfigs);
                var config = configuration.GetSection(sectionKey).Get<SwaggerConfigs>();
                signKeyDict = config.GetSignDict();
            }

            if (!signKeyDict.IsNullOrEmpty())
            {
                var securityRequirements = new OpenApiSecurityRequirement();
                foreach (var item in signKeyDict)
                {
                    var name = item.Key;
                    var des = item.Value;
                    if (!name.IsNullOrEmpty())
                    {
                        // 添加自定义请求头
                        c.AddSecurityDefinition(name, new OpenApiSecurityScheme
                        {
                            Name = name,
                            Description = des,
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                        });

                        securityRequirements.Add(
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = name,
                                },
                            }, new string[] { });
                    }
                }

                if (!securityRequirements.IsNullOrEmpty())
                {
                    c.AddSecurityRequirement(securityRequirements);
                }
            }
        }


        /// <summary>
        ///     UseSwagger And UseSwaggerUI
        /// </summary>
        /// <param name="app"></param>
        /// <param name="assemblyName"></param>
        /// <param name="apiVersion"></param>
        /// <param name="swaggerOptionsAction"></param>
        /// <param name="swaggerUIOptionsAction"></param>
        public static void UseSwaggerConfig(this IApplicationBuilder app, string assemblyName, string apiVersion = "v1",
            Action<SwaggerOptions> swaggerOptionsAction = null, Action<SwaggerUIOptions> swaggerUIOptionsAction = null)
        {
            //  XmlHelper.Instance.Init();


            app.Use(async (context, next) =>
            {
                if (ConfigItems.HiddenSwagger && context.Request.Path.StartsWithSegments("/swagger"))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                if (ConfigItems.IsOnline && !ConfigItems.SwaggerOnlineDebug)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                await next();
            });


            app.UseSwagger(c => { swaggerOptionsAction?.Invoke(c); });


            app.UseSwaggerUI(c =>
            {
                c.DefaultModelExpandDepth(-1);
                c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", assemblyName);
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true"); // 保留授权信息

                //c.RoutePrefix = string.Empty;
                swaggerUIOptionsAction?.Invoke(c);
            });
        }
    }
}
