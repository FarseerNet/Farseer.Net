using System.Collections.Generic;
using Farseer.net.AutoMapper.Test.Entity;
using FS;
using FS.DI;
using FS.Extends;
using NUnit.Framework;

namespace Farseer.net.AutoMapper.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            FarseerApplication.Run<Startup>().Initialize();
        }

        [Test]
        public void Test1()
        {
            IocManager.Instance.Resolve<Test>().Console();

            var lst = new List<UserPO>();
            lst.Add(item: new UserPO { Id = 1, UserName = "张三" });

            var vo    = new UserPO { Id = 1, UserName = "张三" }.Map<UserListVO>();
            var lstVo = lst.Map<UserListVO>();

            Assert.IsNotNull(anObject: vo);
            Assert.IsTrue(condition: lstVo.Count == 1);
        }
    }
}