using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Extensions.@internal;

namespace Swashbuckle.AspNetCore.HideApi.Middleware
{
    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    //public class NonAuthAttribute : Attribute
    //{
    //    public NonAuthAttribute(bool noauth = true)
    //    {
    //        NoAuth = noauth;
    //    }

    //    public bool NoAuth { get; }
    //}

    public abstract class SwaggerSignValidMiddleware
    {
        protected readonly RequestDelegate _next;

        protected readonly Dictionary<string, string> _signKeyDict;


        public SwaggerSignValidMiddleware(RequestDelegate next, Dictionary<string, string> signKeyDict)
        {
            _next = next;
            _signKeyDict = signKeyDict ?? new Dictionary<string, string>();
        }

        protected virtual List<string> AllowedPaths => new() { "/api" };

        protected abstract bool CheckValidData(HttpContext context, Dictionary<string, string> dict);


        public async Task InvokeAsync(HttpContext context)
        {

            if (!IsPathAllowed(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // 判断是否来自 Swagger 页面的请求
            var isSwaggerPage = IsInSwaggerPage(context);
            if (!isSwaggerPage)
            {
                await _next(context);
                return;
            }

            // 执行验证逻辑
            var isAuth = await DoCheckAsync(context);
            if (isAuth)
            {
                await _next(context);
                return;
            }

            // 如果验证失败，确保不会重复发送响应
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or missing signature");
            }

        }

        protected virtual bool IsPathAllowed(PathString path)
        {
            return AllowedPaths.Any(p => path.StartsWithSegments(p));
        }

        protected virtual bool IsInSwaggerPage(HttpContext context)
        {
            var refer = GetHeaderData(context.Request, "Referer");
            if (refer.IsNullOrEmpty())
            {
                return false;
            }
            if (!refer.Contains("swagger", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  需要额外自行实现
        ///     检查当前请求是否标记了 <see cref="NonAuthAttribute" />  属性。
        ///     如果标记了 <see cref="NonAuthAttribute" />，则跳过验证。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool HasNonAuth(HttpContext context)
        {
            //// 获取当前请求的控制器和方法
            //var endpoint = context.GetEndpoint();
            //if (endpoint == null)
            //{
            //    return true;
            //}

            //// 检查控制器或方法是否标记了 [NonAuth] 属性
            //var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            //if (controllerActionDescriptor != null)
            //{
            //    var nonAuthAttrs = controllerActionDescriptor.GetControllerAndActionAttributes<NonAuthAttribute>().OfType<NonAuthAttribute>().ToList();
            //    if (!nonAuthAttrs.IsNullOrEmpty())
            //    {
            //        return true;
            //    }

            //    return false;

            //}

            return false;
        }



        /// <summary>
        ///     获取签名相关的数据。
        ///     如果某些头（如 Ts 或 Timestamp）未提供，则自动填充当前时间戳。
        /// </summary>
        protected virtual Dictionary<string, string> TryGetValidData(HttpContext context)
        {
            var signData = new Dictionary<string, string>();
            if (!_signKeyDict.IsNullOrEmpty())
            {
                foreach (var item in _signKeyDict)
                {
                    var key = item.Key;
                    var data = GetHeaderData(context.Request, key);
                    if (data.IsNullOrEmpty())
                    {
                        if (key == "Ts")
                        {
                            data = ConvertToUnixTime(DateTime.Now).ToString();
                            context.Request.Headers["Ts"] = data;
                        }

                        if (key == "Timestamp")
                        {
                            data = ConvertToUnixTime(DateTime.Now).ToString();
                            context.Request.Headers["Timestamp"] = data;
                        }
                    }

                    signData.Add(key, data);
                }
            }

            return signData;
        }

        protected virtual async Task<bool> DoCheckAsync(HttpContext context)
        {
            if (IsPathAllowed(context.Request.Path))
            {
                // 获取当前请求的控制器和方法
                var endpoint = context.GetEndpoint();
                if (endpoint == null)
                {
                    return await Task.FromResult(true);
                }

                var hasNonAuth = HasNonAuth(context);
                if (hasNonAuth)
                {
                    return await Task.FromResult(true);
                }
                if (!_signKeyDict.IsNullOrEmpty())
                {
                    var signData = TryGetValidData(context);

                    var flag = CheckValidData(context, signData);
                    return await Task.FromResult(flag);
                }
            }

            return await Task.FromResult(true);
        }

        protected virtual string GetHeaderData(HttpRequest httpRequest, string headerName, string defaultValue = "")
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return defaultValue;
        }

        protected virtual long ConvertToUnixTime(DateTime dateTime, bool milliseconds = true)
        {

            DateTimeOffset dateTimeOffset = new(dateTime);
            if (milliseconds)
            {
                return dateTimeOffset.ToUnixTimeMilliseconds();
            }

            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }


}
