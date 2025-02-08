//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.Extensions.@internal;
//using Swashbuckle.AspNetCore.SwaggerGen;

//namespace Swashbuckle.AspNetCore.HideApi.@internal
//{
//    internal class HiddenSchemasResolver : ISchemaFilter
//    {
//        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//        {
//            var isDeprecated = context.Type.GetCustomAttribute<ObsoleteAttribute>() != null;
//            if (isDeprecated)
//            {
//                // 如果模型是过时的，排除它
//                //schema.Deprecated = true;
//                schema.Properties = null;
//                return;
//            }

//            if (ConfigItems.HiddenSchemas) // When clear swaggerDoc.Paths
//            {
//                try
//                {
//                    schema.Properties = null;
//                    //schema.Deprecated = true;
//                }
//                catch
//                {
//                }
//            }
//        }
//    }

//}
