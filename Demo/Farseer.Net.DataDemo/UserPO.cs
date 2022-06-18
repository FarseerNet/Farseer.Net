using System.Collections.Generic;
using FS.Core.Mapping.Attribute;

namespace Farseer.Net.DataDemo;

/// <summary> 用户 </summary>
public class UserPO
{
    /// <summary> 主键 </summary>
    [Field(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public int? Id { get; set; }

    /// <summary> 用户名称 </summary>
    [Field(Name = "name")]
    public string Name { get; set; }

    /// <summary> 用户年龄 </summary>
    [Field(Name = "age")]
    public int? Age { get; set; }

    /// <summary> 用户全称 </summary>
    [Field(Name = "fullname", StorageType = EumStorageType.Json)]
    public FullNameVO Fullname { get; set; }

    /// <summary> 特长 </summary>
    [Field(Name = "specialty", StorageType = EumStorageType.Array)]
    public List<string> Specialty { get; set; }

    /// <summary> 自定义属性 </summary>
    [Field(Name = "attribute", StorageType = EumStorageType.Json)]
    public Dictionary<string, string> Attribute { get; set; }

    /// <summary> 自定义属性 </summary>
    [Field(Name = "gender")]
    public GenderType? Gender { get; set; }
}

public class FullNameVO
{
    /// <summary> 姓氏 </summary>
    public string FirstName { get; set; }
    /// <summary> 名称 </summary>
    public string LastName { get; set; }
}

public enum GenderType
{
    Man,
    Woman
}