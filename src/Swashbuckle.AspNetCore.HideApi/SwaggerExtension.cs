using System;
using System.Attributes;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.Abstractions;
using Swashbuckle.AspNetCore.Extensions.Internal;

namespace Swashbuckle.AspNetCore.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        ///  Init Config 
        ///  AddSwaggerGen
        ///  Add HiddenApiAttribute And HiddenSwaggerFilter
        /// </summary>
        /// <param name="services"></param>
        /// <param name="title"></param>
        /// <param name="configuration"></param>
        /// <param name="swaggerConfig"></param>
        /// <param name="apiVersion"></param>
        /// <remarks>
        ///     add AddSwaggerGen with DocumentFilter
        ///     1、 <see cref="System.Attributes.HiddenApiAttribute" />
        ///     2、 <see cref="System.Attributes.HiddenSwaggerFilter" />
        ///     add all files like *.xmls in AppContext.BaseDirectory
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, string title, IConfiguration configuration, ISwaggerConfig swaggerConfig = null,
            string apiVersion = "v1")
        {
            if (swaggerConfig != null)
            {
                ConfigItems.InitSwaggerConfig(swaggerConfig);
            }

            ConfigItems.InitConfiguration(configuration);

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();
                c.DocumentFilter<HiddenApiAttribute>();
                c.DocumentFilter<HiddenSwaggerFilter>();
                c.SwaggerDoc(apiVersion, new OpenApiInfo
                {
                    Title = title,
                    Version = apiVersion, 

                });
                var path = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(path))
                    if (".xml".Equals(Path.GetExtension(file)))
                        c.IncludeXmlComments(Path.GetFullPath(file));
            });
            return services;
        }

        /// <summary>
        /// UseSwagger And UseSwaggerUI
        /// </summary>
        /// <param name="app"></param>
        /// <param name="assemblyName"></param>
        /// <param name="apiVersion"></param>
        public static void UseSwaggerConfig(this IApplicationBuilder app, string assemblyName, string apiVersion = "v1")
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", assemblyName);
                //c.RoutePrefix = string.Empty;
            });
        }
    }
}
