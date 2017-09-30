using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Castle.DynamicProxy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CSharp;
using ZTO.Platform.DI;
using ZTO.Test.Commom;
using ZTO.Test.Interface;

namespace ZTO.Platform.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ZtoBootstrapper.Create<StartupModule>().Initialize();
            var testAutoRegister = IocManager.Instance.Resolve<ITestAutoRegister>();
            
        var pg = new ProxyGenerator().CreateClassProxy<TestAutoRegister>(new TestIntercept());
            pg.Write();
        }
    }

    public class TestIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            System.Console.WriteLine("111111111111");
            invocation.Proceed();
            System.Console.WriteLine("222");
        }
    }


}
