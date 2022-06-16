using System;
using System.Collections.Generic;
using FS.Core.Abstract.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Farseer.Net.AspNetCore.DynamicApi
{
    public class DynamicWebApiOptions : IDynamicWebApiOptions
    {
        /// <summary>
        /// API URL的前缀，默认：api
        /// </summary>
        public string DefaultApiPrefix { get; set; } = "api";

        /// <summary>
        /// Ignore MVC Form Binding types.
        /// </summary>
        public List<Type> FormBodyBindingIgnoredTypes { get; set; } = new() { typeof(IFormFile) };

        /// <summary>
        /// 验证设置
        /// </summary>
        public void Valid()
        {
            if (string.IsNullOrEmpty(DefaultApiPrefix)) DefaultApiPrefix = string.Empty;
            if (FormBodyBindingIgnoredTypes == null) throw new ArgumentException($"{nameof(FormBodyBindingIgnoredTypes)} can not be null.");
        }
    }
}