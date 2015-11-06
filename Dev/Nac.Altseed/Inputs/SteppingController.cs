using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Input
{
	public class SteppingController<TAbstract> : Controller<TAbstract>
	{
		class HoldManager
		{
			private int inputTime;
			private int startTime;
			private int span;

			public HoldManager(int startTime, int span)
			{
				this.startTime = startTime;
				this.span = span;
				inputTime = 0;
			}

			public void Update(InputState? state)
			{
				if(state == InputState.Push || state == InputState.Hold)
				{
					inputTime++;
				}
				else
				{
					inputTime = 0;
				}
			}

			public bool IsPush()
			{
				return (inputTime - startTime) % span == 1;
			}
		}

		Controller<TAbstract> controller;
		Dictionary<TAbstract, HoldManager> managers;

		public override IEnumerable<TAbstract> Keys
		{
			get { return controller.Keys; }
		}

		public bool IsChildUpdated { get; set; }

		public SteppingController(Controller<TAbstract> controller)
		{
			this.controller = controller;
			managers = new Dictionary<TAbstract, HoldManager>();
		}

		public void EnableSteppingHold(TAbstract key, int startTime, int span)
		{
			managers[key] = new HoldManager(startTime, span);
		}

		public override InputState? GetState(TAbstract key)
		{
			if(!managers.ContainsKey(key))
			{
				return controller.GetState(key);
			}
			else
			{
				if(managers[key].IsPush())
				{
					return InputState.Push;
				}
				else
				{
					return controller.GetState(key);
				}
			}
		}

		public override void Update()
		{
			foreach(var item in managers)
			{
				item.Value.Update(controller.GetState(item.Key));
			}
			if(IsChildUpdated)
			{
				controller.Update();
			}
		}
	}
}
