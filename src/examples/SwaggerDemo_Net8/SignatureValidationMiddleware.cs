using Microsoft.AspNetCore.Mvc.Controllers;
using SwaggerDemo_Net8.@internal;
using Swashbuckle.AspNetCore.HideApi.Middleware;

namespace SwaggerDemo_Net8
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NonAuthAttribute : Attribute
    {
        public NonAuthAttribute(bool noauth = true)
        {
            NoAuth = noauth;
        }

        public bool NoAuth { get; }
    }

 
    public class SelfSwaggerSignValidMiddleware : SwaggerSignValidMiddleware
    {
        public SelfSwaggerSignValidMiddleware(RequestDelegate next, Dictionary<string, string> signKeyDict) : base(next, signKeyDict)
        {
        }

        protected override bool CheckValidData(HttpContext context, Dictionary<string, string> dict)
        {

            return true;
        }

        /// <summary>
        ///  需要额外自行实现
        ///     检查当前请求是否标记了 <see cref="NonAuthAttribute" />  属性。
        ///     如果标记了 <see cref="NonAuthAttribute" />，则跳过验证。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool HasNonAuth(HttpContext context)
        {
            // 获取当前请求的控制器和方法
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                return true;
            }

            // 检查控制器或方法是否标记了 [NonAuth] 属性
            var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor != null)
            {
                var nonAuthAttrs = controllerActionDescriptor.GetControllerAndActionAttributes<NonAuthAttribute>().OfType<NonAuthAttribute>().ToList();
                if (!nonAuthAttrs.IsNullOrEmpty())
                {
                    return true;
                }

                return false;

            }

            return false;
        }

    }



}
