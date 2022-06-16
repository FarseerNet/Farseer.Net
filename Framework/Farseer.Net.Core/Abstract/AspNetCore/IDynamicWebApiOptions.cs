using System;
using System.Collections.Generic;

namespace FS.Core.Abstract.AspNetCore;

public interface IDynamicWebApiOptions
{
    /// <summary>
    /// API URL的前缀，默认：api
    /// </summary>
    string DefaultApiPrefix { get; set; }
    /// <summary>
    /// Ignore MVC Form Binding types.
    /// </summary>
    List<Type> FormBodyBindingIgnoredTypes { get; set; }
    /// <summary>
    /// 验证设置
    /// </summary>
    void Valid();
}