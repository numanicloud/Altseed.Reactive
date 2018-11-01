using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			var test = new Ui.MarkupTextViewerTest();
			test.Run();
		}

		private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			throw e.Exception;
		}
	}
}
