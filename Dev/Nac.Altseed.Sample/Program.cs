using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sample_cs;

namespace Nac.Altseed.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var samples = new ISample[]
			{
				new Controller.Controller_Keyboard(),
				new Controller.Controller_Bundle(),
				new Controller.Controller_Stepping(),
				new Selector.Selector_Basic(),
			};

			var browser = new SampleBrowser(samples);
			browser.Run();
		}
	}
}
