using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swashbuckle.NetFx.HideApi.Internals
{
    internal class ConfigItems
    {
        protected const string TrueString = "1";
        protected const string FalseString = "0";
        protected const string ZeroString = "0";
        protected const int ZeroInt = 0;

        public static bool HiddenSwagger => GetAppSetting("HiddenSwagger", FalseString).Equals(TrueString);
        public static bool ShowHiddenApi => GetAppSetting("ShowHiddenApi", FalseString).Equals(TrueString);

        public static int SysAppId => GetConfigValue("AppId", ZeroInt, false);


        internal static string SysAppCode => GetConfigValue("AppCode", false);

        internal static string SysAppName => GetConfigValue("AppName", false);
        internal static string SysAppVer => GetConfigValue("AppVersion", "v1", false);


        /// <summary>根据键名获取连接字符串</summary>
        /// <param name="connKey"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConnStringValue(string connKey, bool isThrow = true)
        {
            return GetConnStringValue(connKey, "", isThrow);
        }

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, bool isThrow = true)
        {
            return GetConfigValue(configKey, "", isThrow: isThrow);
        }

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="isThrow"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, string defaultValue = "", bool isThrow = true)
        {
            return GetAppSetting(configKey, defaultValue, isThrow);
        }
        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static int GetConfigValue(string configKey, int defaultValue = 0, bool isThrow = true)
        {
            return GetAppSetting(configKey, defaultValue.ToString(), isThrow).ToInt(defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static bool GetConfigValue(string configKey, bool defaultValue = false, bool isThrow = true)
        {
            return GetAppSetting(configKey, defaultValue ? "1" : "0", isThrow) == "1";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        static string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = false)
        {
            return ReadFromConfig((key) =>
            {
                var configValue = ConfigurationManager.AppSettings.Get(key);
                return configValue;
            }, configKey, defaultValue, isThrow);

        }

        /// <summary>
        /// </summary>
        /// <param name="connKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConnStringValue(string connKey, string defaultValue, bool isThrow = true)
        {
            return ReadFromConfig((key) =>
            {
                string configValue = ConfigurationManager.ConnectionStrings[connKey]?.ToString();
                return configValue;
            }, connKey, defaultValue, isThrow);

        }

        static string ReadFromConfig(Func<string, string> func, string configKey, string defaultValue = "", bool isThrow = false)
        {
            if (string.IsNullOrEmpty(configKey)) return defaultValue;
            var configValue = string.Empty;
            try
            {
                configValue = func.Invoke(configKey);
                if (string.IsNullOrEmpty(configValue) && isThrow)
                {
                    throw new ArgumentException($"未能找到 【{configKey}】节点的相关配置");
                }
            }
            catch (Exception ex)
            {
                if (isThrow)
                    throw new Exception($"查找【{configKey}】节点的相关配置发生异常！", ex);
            }
            if (string.IsNullOrEmpty(configValue) && !string.IsNullOrEmpty(defaultValue)) configValue = defaultValue;

            return configValue;
        }


    }
}
