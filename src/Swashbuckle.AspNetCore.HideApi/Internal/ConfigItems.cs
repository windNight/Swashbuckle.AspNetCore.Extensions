using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Extensions.Abstractions;

namespace Swashbuckle.AspNetCore.Extensions.Internal
{

    internal class ConfigItems : ConfigItemsBase
    {
        /// <summary>
        /// 初始化 ISwaggerConfig
        /// </summary>
        /// <param name="swaggerConfig"></param>
        public static void InitSwaggerConfig(ISwaggerConfig swaggerConfig)
        {
            SwaggerConfig = swaggerConfig;
        }

        /// <summary>
        /// 初始化 IConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public static void InitConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary> 控制是否显示HiddenApi，默认为否 </summary>
        public static bool ShowHiddenApi
        {
            get
            {
                if (SwaggerConfig != null)
                {
                    return SwaggerConfig.GetShowHiddenApiConfig();
                }
                return GetConfigValue("AppSettings:ShowHiddenApi", false, false);
            }
        }

        /// <summary> 控制是否隐藏Swagger，默认为否 </summary>
        public static bool HiddenSwagger
        {
            get
            {
                if (SwaggerConfig != null)
                {
                    return SwaggerConfig.GetHiddenSwaggerConfig();
                }
                return GetConfigValue("AppSettings:HiddenSwagger", false, false);
            }
        }


    }

    internal class ConfigItemsBase
    {
        /// <summary> 优先级高于配置 </summary>
        protected static ISwaggerConfig SwaggerConfig { get; set; }

        /// <summary> 读取默认配置 </summary>
        protected static IConfiguration Configuration { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, bool isThrow = true)
        {
            return GetConfigValue(configKey, "", isThrow);
        }


        private const string TrueString = "1", FalseString = "0", ZeroString = "0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static bool GetConfigValue(string configKey, bool defaultValue = false, bool isThrow = true) => GetConfigValue(configKey, FalseString, isThrow) == TrueString;

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, string defaultValue, bool isThrow = true)
        {
            string configValue;
            try
            {
                if (Configuration == null)
                {
                    throw new ArgumentException($"Configuration 未初始化！");
                }
                configValue = Configuration[configKey];

                if (string.IsNullOrEmpty(configValue))
                {
                    if (isThrow)
                    {
                        if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
                        throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置");
                    }

                    configValue = defaultValue;
                }
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
                    throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置", ex);
                }
                configValue = defaultValue;
            }

            return configValue;
        }


    }
}
