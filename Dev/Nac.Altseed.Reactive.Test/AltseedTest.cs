using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;

namespace Nac.Altseed.Reactive.Test
{
    enum Control
    {
        Left, Right, Up, Down, Decide, Cancel
    }

	class AltseedTest
	{
		public void Run()
		{
            var syncContext = new UpdatableSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);

			Engine.Initialize(this.GetType().Name, 640, 480, new EngineOption());
			Engine.File.AddRootDirectory("Resources");

			OnStart();

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

		protected virtual void OnStart()
		{
		}

        protected KeyboardController<Control> CreateController()
        {
            var controller = new KeyboardController<Control>();
            controller.BindDirection(Control.Left, Control.Right, Control.Up, Control.Down);
            controller.BindKey(Keys.Z, Control.Decide);
            controller.BindKey(Keys.X, Control.Cancel);
            return controller;
        }
	}
}
