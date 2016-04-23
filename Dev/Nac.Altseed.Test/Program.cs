using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Altseed.Test.Selector;

namespace Nac.Altseed.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AltseedTest test = new ScrollAndCenterPositionTest();
            test.Run();
        }
    }
}
