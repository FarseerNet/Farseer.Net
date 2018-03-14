// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-11-17 20:06
// ********************************************

using System.Data;
using FS.Core.Mapping.Attribute;

namespace Farseer.Net.Data.Console
{
    /// <summary>
    ///     Sp供应商账号
    /// </summary>
    public class SpAccountPO
    {
        /// <summary>
        ///     主键
        /// </summary>
        [Field(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }
        /// <summary>
        ///     账号
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string Account { get; set; }
        /// <summary>
        ///     密码
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string Password { get; set; }
        /// <summary>
        ///     发送短信接口地址
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string SendApiUrl { get; set; }
        /// <summary>
        ///     接收网关地址
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string GatewayApiUrl { get; set; }
        /// <summary>
        ///     接收上行地址
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string ReplyApiUrl { get; set; }
        /// <summary>
        ///     上行短信确认地址
        /// </summary>
        [Field(DbType = DbType.AnsiString)]
        public string ReplyRespApiUrl { get; set; }
        /// <summary>
        ///     是否加入随即选择
        /// </summary>
        public bool IsRandom { get; set; }
        /// <summary>
        ///     运单号发送
        /// </summary>
        public bool IsUseBillCode { get; set; }
        /// <summary>
        /// 短号
        /// </summary>
        public bool IsUseShort { get; set; }
        /// <summary>
        ///     权重
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        ///     是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        ///     备注
        /// </summary>
        public string Remak { get; set; }
    }
}