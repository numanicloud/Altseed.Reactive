using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
	public static class Debug
	{
		public static void WriteWarning(object sender, string message)
		{
			var actualMessage = $"Warning in {sender.GetType().FullName}: {message}";
			Trace.WriteLine(actualMessage);
			Engine.Logger.WriteLine(actualMessage);
		}
	}
}
