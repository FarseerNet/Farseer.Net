using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FS.Utils.Common;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class Extend
    {
        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue"> 要转换的值 </param>
        /// <param name="defType"> 要转换的对象的类型 </param>
        public static object ConvertType(this object sourceValue, Type defType) => ConvertHelper.ConvertType(sourceValue: sourceValue, returnType: defType);


        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue"> 要转换的源对象 </param>
        /// <param name="defValue"> 转换失败时，代替的默认值 </param>
        /// <typeparam name="T"> 基本类型 </typeparam>
        public static T ConvertType<T>(this object sourceValue, T defValue = default) => ConvertHelper.ConvertType(sourceValue: sourceValue, defValue: defValue);

        /// <summary>
        /// 克隆对象
        /// </summary>
        public static TEntity Clone<TEntity>(this TEntity source)
        {
            if (source == null) return default;
            return JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(source), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });

            // IFormatter formatter = new BinaryFormatter();
            // Stream     stream    = new MemoryStream();
            // using (stream)
            // {
            //     formatter.Serialize(stream, source);
            //     stream.Seek(0, SeekOrigin.Begin);
            //     return (TEntity)formatter.Deserialize(stream);
            // }
        }
    }
}