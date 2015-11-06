using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Test
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
			Engine.Initialize(name, 640, 480, new EngineOption());

			OnInitialize();

			while(Engine.DoEvents())
			{
				Engine.Update();
				OnUpdate();
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
