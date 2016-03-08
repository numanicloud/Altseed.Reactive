using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.Test
{
    public enum Control
    {
        Left, Right, Up, Down, Decide, Cancel, Sub
    }

	class AltseedTest
	{
		public int TimeCount { get; set; }

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
				++TimeCount;
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

        protected Controller<Control> CreateController()
        {
            var controller = new KeyboardController<Control>();
            controller.BindDirection(Control.Left, Control.Right, Control.Up, Control.Down);
            controller.BindKey(Keys.Z, Control.Decide);
            controller.BindKey(Keys.X, Control.Cancel);
			controller.BindKey(Keys.C, Control.Sub);

			var stepping = new SteppingController<Control>(controller);
			stepping.IsChildUpdated = true;
			stepping.EnableSteppingHold(Control.Down, 10, 7);
			stepping.EnableSteppingHold(Control.Up, 10, 7);
			stepping.EnableSteppingHold(Control.Left, 10, 7);
			stepping.EnableSteppingHold(Control.Right, 10, 7);

			return stepping;
        }
	}
}
