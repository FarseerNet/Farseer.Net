using System.Reflection;
using FS.Core.Mapping.Attribute;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FS.Data.Map
{
    /// <summary>
    ///     这里需要重新获取PO的字段名称，不能直接读类属性名称或通过JsonProperty特性（会产生与业务冲突问题）
    /// </summary>
    internal class JsonFieldNameContractResolver : DefaultContractResolver
    {
        private readonly SetDataMap _setDataMap;

        internal JsonFieldNameContractResolver(SetDataMap setDataMap)
        {
            _setDataMap = setDataMap;
        }

        /// <summary>
        ///     重新赋值类属性的字段名称
        /// </summary>
        /// <param name="member"> </param>
        /// <param name="memberSerialization"> </param>
        /// <returns> </returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member: member, memberSerialization: memberSerialization);

            var propertyInfo = _setDataMap.PhysicsMap.GetState(propertyName: member.Name);
            if (propertyInfo.Key != null)
            {
                jsonProperty.PropertyName = propertyInfo.Value.Field.Name;
                jsonProperty.Ignored      = propertyInfo.Value.Field.StorageType == EumStorageType.Ignore;
            }

            return jsonProperty;
        }
    }
}