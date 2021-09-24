using FS.Core.Mapping.Attribute;

namespace Farseer.Net.DataDemo
{
    public class UserCoinsPO
    {
        [Field(Name = "ID", IsPrimaryKey = true, IsDbGenerated = true)]
        public int? ID { get; set; }

        public decimal? Coins { get; set; }

        [Field(Name = "coins_type")]
        public EumCoinsType? CoinsType { get; set; }

        [Field(Name = "parent_agenter_name")]
        public string ParentAgenterName { get; set; }

        public decimal? Quota { get; set; }

        [Field(Name = "user_name")]
        public string UserName { get; set; }
    }
}