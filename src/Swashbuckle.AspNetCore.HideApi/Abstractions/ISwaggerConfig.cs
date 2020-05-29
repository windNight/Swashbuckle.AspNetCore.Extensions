using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.Extensions.Abstractions
{

    public interface ISwaggerConfig
    {
        /// <summary>
        /// 自定义获取是否显示HiddenApi的配置
        /// </summary>
        /// <returns></returns>
        bool GetShowHiddenApiConfig();

        /// <summary>
        /// 自定义获取是否显示swagger的配置
        /// </summary>
        /// <returns></returns>
        bool GetHiddenSwaggerConfig();

    }
}
