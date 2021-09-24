// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-08-17 11:08
// ********************************************

using FS.Cache;

namespace FS.Utils.Common
{
    public class EntityHelper
    {
        /// <summary>
        ///     查找对象属性值
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <param name="entity"> 当前实体类 </param>
        /// <param name="propertyName"> 属性名 </param>
        /// <param name="defValue"> 默认值 </param>
        public static T GetValue<TEntity, T>(TEntity entity, string propertyName, T defValue = default) where TEntity : class
        {
            if (entity == null) return defValue;
            foreach (var property in entity.GetType().GetProperties())
            {
                if (property.Name != propertyName) continue;
                if (!property.CanRead) return defValue;
                return ConvertHelper.ConvertType(sourceValue: PropertyGetCacheManger.Cache(key: property, instance: entity), defValue: defValue);
            }

            return defValue;
        }
    }
}