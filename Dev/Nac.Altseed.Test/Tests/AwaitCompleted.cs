using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test
{
	class AwaitCompleted : AltseedTest
	{
		private async Task RunAsync()
		{
			var observable = Observable.Range(0, 10);
			await observable.TakeWhile(x => x > 100)
				.Do(x => Console.WriteLine(x), () => Console.WriteLine("Completed"));
		}

		protected override void OnStart()
		{
			RunAsync().Wait();
			Console.WriteLine("Finish!");
		}
	}
}
