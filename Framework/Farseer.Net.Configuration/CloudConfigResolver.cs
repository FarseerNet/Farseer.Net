// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 11:17
// ********************************************

using System;
using FS.Configuration.Format;

namespace FS.Configuration
{
    /// <summary>
    /// 配置中心的解析
    /// </summary>
    public class CloudConfigResolver : IConfigResolver
    {
        /// <inherit/>
        public void Append(Type configType) { throw new NotImplementedException(); }
        /// <inherit/>
        public void Load() { throw new NotImplementedException(); }
        /// <inherit/>
        public void Save() { throw new NotImplementedException(); }
        /// <inherit/>
        public TConfigEntity Get<TConfigEntity>() where TConfigEntity : class, IFarseerConfig, new() { throw new NotImplementedException(); }
        /// <inherit/>
        public void Set<TConfigEntity>(TConfigEntity configEntity) where TConfigEntity : IFarseerConfig, new() { throw new NotImplementedException(); }
    }
}