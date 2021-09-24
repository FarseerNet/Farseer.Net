using FS.Mapper;

namespace Farseer.net.AutoMapper.Test.Entity
{
    /// <summary>
    ///     会员账号实体
    /// </summary>
    [Map(typeof(UserListVO))]
    public class UserPO
    {
        /// <summary> </summary>
        [MapFieldAttribute(IsIgnore = true)]
        public int? Id { get; set; }

        /// <summary> 用户账号 </summary>
        [MapFieldAttribute(FromName = "Name")]
        public string UserName { get; set; }
    }
}