using System.ComponentModel.DataAnnotations;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    ///     类型
    /// </summary>
    public enum EumCoinsType
    {
        /// <summary> 未指定 </summary>
        [Display(Name = "未指定")]
        None = -1,
        Credit = 0,
        Cash = 1
    }
}