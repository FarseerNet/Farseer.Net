using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Farseer.Net.AspNetCore.DynamicApi
{
    public class DynamicWebApiOptions
    {
        public DynamicWebApiOptions()
        {
            FormBodyBindingIgnoredTypes = new List<Type> { typeof(IFormFile) };
            DefaultHttpVerb             = "GET";
            DefaultApiPrefix            = "api";
        }

        /// <summary>
        /// 默认HttpMethod：GET
        /// </summary>
        public string DefaultHttpVerb { get; set; }

        public string DefaultAreaName { get; set; }

        /// <summary>
        /// API URL的前缀，默认：api
        /// </summary>
        public string DefaultApiPrefix { get; set; }

        /// <summary>
        /// Ignore MVC Form Binding types.
        /// </summary>
        public List<Type> FormBodyBindingIgnoredTypes { get; set; }

        /// <summary>
        /// 验证设置
        /// </summary>
        public void Valid()
        {
            if (string.IsNullOrEmpty(DefaultHttpVerb)) DefaultHttpVerb   = "GET";
            if (string.IsNullOrEmpty(DefaultAreaName)) DefaultAreaName   = string.Empty;
            if (string.IsNullOrEmpty(DefaultApiPrefix)) DefaultApiPrefix = string.Empty;

            if (FormBodyBindingIgnoredTypes == null) throw new ArgumentException($"{nameof(FormBodyBindingIgnoredTypes)} can not be null.");
        }
    }
}