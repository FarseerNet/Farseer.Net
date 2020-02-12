// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-29 17:06
// ********************************************

using System.Collections.Generic;
using FS.Data;
using FS.Data.Map;

namespace Farseer.Net.Data.Console
{
    /// <summary>
    ///     新的数据库结构
    /// </summary>
    public class TestContext : DbContext<TestContext>
    {
        public TestContext() : base("test", false) { }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map["UserCoins"].SetName("account_user_coins");
        }
        public TableSet<UserCoinsPO> UserCoins { get; set; }

    }
}