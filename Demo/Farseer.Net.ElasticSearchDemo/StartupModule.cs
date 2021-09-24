﻿using FS.ElasticSearch;
using FS.Modules;

namespace Farseer.Net.ElasticSearchDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(ElasticSearchModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
        }
    }
}