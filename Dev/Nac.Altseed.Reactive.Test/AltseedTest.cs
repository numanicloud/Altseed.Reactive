using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.Test
{
	class AltseedTest
	{
		private string name;

		public AltseedTest(string name)
		{
			this.name = name;
		}

		public void Run()
		{
            var syncContext = new UpdatableSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);

			Engine.Initialize(name, 640, 480, new EngineOption());
			Engine.File.AddRootDirectory("Resources");

			OnInitialize();

			while(Engine.DoEvents())
			{
				Engine.Update();
                UpdateManager.Instance.Update();
                OnUpdate();
                syncContext.Update();
			}

			OnTerminate();
			Engine.Terminate();
		}

		protected virtual void OnTerminate()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnInitialize()
		{
		}
	}
}
