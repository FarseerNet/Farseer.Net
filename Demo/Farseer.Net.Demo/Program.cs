using System;
using System.Threading.Tasks;
using FS.Core.AOP;
using FS.Core.Http;

namespace Farseer.Net.Demo
{
    class Program
    {
        [WhileTrue(100)]
        private static void OutputText()
        {
            Console.WriteLine(DateTime.Now);
        }

        static async Task Main(string[] args)
        {
            OutputText();

            await HttpGet.GetAsync("http://www.baidu.com");

            var result = Calc(5, 6);
            Console.WriteLine($"计算结果：{result}");
            Console.WriteLine(">>>>>>>>>>>>>>方法拦截测试完毕\r\n");


            PropertyTest = -1;
            Console.WriteLine(">>>>>>>>>>>>>>属性拦截测试(setter)完毕\r\n");


            var x = PropertyTest;
            Console.WriteLine(">>>>>>>>>>>>>>属性拦截测试(getter)完毕\r\n");

            Console.ReadKey();
        }


        /// <summary>
        /// 方法拦截测试 + 异常处理
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [HowToUse, ExceptionHandle]
        private static int Calc(int x, int y)
        {
            int a = 1;
            int b = 0;
            int c = a / b;

            return x + y;
        }

        private static int _propertyTest;

        /// <summary>
        /// 属性拦截测试
        /// 注：可以标记在整个属性上，也可以分别单独标记在 【getter】 或者 【setter】 上
        /// </summary>
        [HowToUse, ExceptionHandle]
        private static int PropertyTest
        {
            get => _propertyTest;
            [HowToUse]
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"属性值必须大于0");
                }

                _propertyTest = value;
            }
        }
    }
}