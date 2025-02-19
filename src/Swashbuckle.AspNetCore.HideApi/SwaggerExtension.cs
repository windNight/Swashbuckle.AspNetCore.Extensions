using System;
using System.Attributes;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.Abstractions;
using Swashbuckle.AspNetCore.Extensions.@internal;
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
            bool paramUpperCamelCase = true, Dictionary<string, string> signKeyDict = null)
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

                            securityRequirements.Add(new OpenApiSecurityScheme
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



            });
            return services;
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
