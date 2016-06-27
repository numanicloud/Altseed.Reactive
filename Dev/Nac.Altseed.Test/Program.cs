using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Altseed.Test.Selector;
using Nac.Altseed.Test.Tests;

namespace Nac.Altseed.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AltseedTest test = new SelectorForSelector();
            test.Run();
        }
    }
}
