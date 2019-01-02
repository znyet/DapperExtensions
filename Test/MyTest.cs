using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class MyTest  //DapperExtensions测试类
    {
        [Test]
        public void GetName()
        {
            Assert.IsNotNull("李四");
        }

        [Test]
        public void GetAddress()
        {
            Assert.Pass("漳州市");
        }
    }

}
