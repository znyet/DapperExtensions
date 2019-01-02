using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Test
{
    [Ignore("")]
    [TestFixture]
    public class MyTestAsync
    {
        [Test]
        public void GetNameAsync()
        {
            Assert.IsNotNull("李四");
        }
    }
}
