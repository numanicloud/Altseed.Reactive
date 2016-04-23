using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Study
{
	class DisposeScrollSelector : AltseedTest
	{
		private Scene scene;

		protected override void OnStart()
		{
			scene = new Scene();
			scene.AddLayer(new Layer2D());
			Engine.ChangeScene(scene);
			RunAsync();
		}

		private async Task RunAsync()
		{
			var selector2 = new ScrollingSelector<int, Control>(CreateController())
			{
				BoundLines = 6,
				LineSpan = 40,
				LineWidth = 120,
				IsControllerUpdated = true,
			};
			selector2.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector2.AddChoice(0, new TextureObject2D());
			scene.AddLayer(selector2);

			Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

			await selector2.OnDecide.FirstAsync().ToTask();

			Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

			selector2.Dispose();
		}

		protected override void OnUpdate()
		{
			if (TimeCount == 600)
			{
				GC.Collect();
			}
		}
	}
}
