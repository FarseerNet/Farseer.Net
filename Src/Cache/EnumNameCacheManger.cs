using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Cache
{
    /// <summary>
    ///     枚举显示中文名称缓存
    /// </summary>
    public class EnumNameCacheManger : AbsCacheManger<string, string>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly Enum _eum;
        private readonly Type _enum;
        private readonly string _enumName;

        private EnumNameCacheManger(Enum eum)
        {
            _eum = eum;
            _enum = eum.GetType();
            _enumName = eum.ToString();
            Key = string.Format("{0}.{1}", _enum.FullName, _enumName);
        }

        protected override string SetCacheLock()
        {
            if (_eum == null) { return ""; }
            foreach (var fieldInfo in _enum.GetFields())
            {
                //判断名称是否相等   
                if (fieldInfo.Name != _enumName) continue;

                //反射出自定义属性   
                foreach (Attribute attr in fieldInfo.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称
                    var dscript = attr as DisplayAttribute;
                    if (dscript == null) { continue; }
                    lock (LockObject) { if (!CacheList.ContainsKey(Key)) { CacheList.Add(Key, dscript.Name); } }
                    return dscript.Name;
                }
            }

            //如果没有检测到合适的注释，则用默认名称   
            return _enumName;
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="eum">枚举</param>
        public static string Cache(Enum eum)
        {
            return new EnumNameCacheManger(eum).GetValue();
        }
    }
}