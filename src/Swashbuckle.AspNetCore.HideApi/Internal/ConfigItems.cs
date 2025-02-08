using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Extensions.Abstractions;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal class SwaggerConfigs
    {
        public bool ShowHiddenApi { get; set; } = false;
        public bool HiddenSwagger { get; set; } = true;
        public bool HiddenSchemas { get; set; } = false;
        public bool ShowTestApi { get; set; } = false;
        public bool ShowSysApi { get; set; } = false;
        public bool IsManageApp { get; set; } = false;
    }


    internal class ConfigItems : ConfigItemsBase
    {
        /// <summary> 控制是否显示HiddenApi，默认为否 </summary>
        public static bool ShowHiddenApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowHiddenApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowHiddenApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowHiddenApi", false, false);

                return configValue;
            }
        }


        /// <summary> 控制是否隐藏Swagger，默认为否 </summary>
        public static bool HiddenSwagger
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetHiddenSwaggerConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSwagger;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSwagger", false, false);
                return configValue;
            }
        }

        /// <summary> 控制是否隐藏Swagger Schemas，默认为否 </summary>

        public static bool HiddenSchemas
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetHiddenSchemasConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSchemas;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSchemas", false, false);

                return configValue;
            }
        }

        /// <summary> 控制是否显示TestApi，默认为否 </summary>
        public static bool ShowTestApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowTestApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowTestApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowTestApi", false, false);
                return configValue;
            }
        }

        /// <summary> 控制是否显示SysApi，默认为否 </summary>
        public static bool ShowSysApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowTestApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowSysApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowSysApi", false, false);
                return configValue;
            }
        }

        public static string SysAppId =>
            GetConfigValue("AppSettings:AppId", ZeroString, false);

        public static string SysAppCode =>
            GetConfigValue("AppSettings:AppCode", ZeroString, false);

        public static string SysAppName =>
            GetConfigValue("AppSettings:AppName", ZeroString, false);

        public static SwaggerConfigs SwaggerConfigs => GetSectionValue<SwaggerConfigs>();

        /// <summary>
        ///     初始化 ISwaggerConfig
        /// </summary>
        /// <param name="swaggerConfig"></param>
        public static void InitSwaggerConfig(ISwaggerConfig swaggerConfig)
        {
            SwaggerConfigImpl = swaggerConfig;
        }

        /// <summary>
        ///     初始化 IConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public static void InitConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }

    internal class ConfigItemsBase
    {
        protected const string TrueString = "1", FalseString = "0", ZeroString = "0";

        protected static string[] TrueStrings = { "1", "true" }, FalseStrings = { "0", "false" };

        /// <summary> 优先级高于配置 </summary>
        protected static ISwaggerConfig SwaggerConfigImpl { get; set; }

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

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static bool GetConfigValue(string configKey, bool defaultValue = false, bool isThrow = true)
        {
            var configValue = GetConfigValue(configKey, "", isThrow).ToLower();
            if (configValue.IsNullOrEmpty())
                return defaultValue;

            if (TrueStrings.Contains(configValue))
            {
                return true;
            }

            if (FalseStrings.Contains(configValue))
            {
                return false;
            }

            if (isThrow)
            {
                throw new ArgumentOutOfRangeException("configKey",
                    $"configKey({configKey}) is not in TrueStrings({string.Join(",", TrueStrings)}) or FalseStrings({string.Join(",", FalseStrings)}) ");
            }

            return defaultValue;
        }


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
                    throw new ArgumentException("Configuration 未初始化！");
                }

                configValue = Configuration[configKey];

                if (configValue.IsNullOrEmpty())
                {
                    if (isThrow)
                    {
                        if (!defaultValue.IsNullOrEmpty()) return defaultValue;
                        throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置");
                    }

                    configValue = defaultValue;
                }
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    if (!defaultValue.IsNullOrEmpty()) return defaultValue;
                    throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置", ex);
                }

                configValue = defaultValue;
            }

            return configValue;
        }

        protected static T GetSectionValue<T>(
            string sectionKey = "",
            T defaultValue = default,
            bool isThrow = false)
            where T : class, new()
        {
            if (sectionKey.IsNullOrEmpty())
                sectionKey = typeof(T).Name;
            return GetSectionConfigValue(sectionKey, defaultValue, isThrow);
        }

        protected static T GetSectionConfigValue<T>(
            string sectionKey,
            T defaultValue = default,
            bool isThrow = false)
        {
            if (defaultValue == null)
                defaultValue = default;
            var obj = defaultValue;
            try
            {
                if (Configuration == null)
                    return defaultValue;
                obj = Configuration.GetSection(sectionKey).Get<T>();
                return obj ?? defaultValue;
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    //LogHelper.Error(string.Format("GetSection({0}) configValue({1}) defaultValue({2}) isThrow({3}) handler error {4}", (object)sectionKey, (object)obj, (object)defaultValue, (object)isThrow, (object)ex.Message), ex);
                    throw;
                }
            }

            return defaultValue;
        }
    }
}
