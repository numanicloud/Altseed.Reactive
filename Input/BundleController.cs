using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Input
{
	public class BundleController<TAbstractKey> : Controller<TAbstractKey>
	{
		List<Controller<TAbstractKey>> controllers;

		public override IEnumerable<TAbstractKey> Keys
		{
			get { return controllers.SelectMany(x => x.Keys).ToArray(); }
		}

		public bool IsChildrenUpdated { get; set; }

		public BundleController(params Controller<TAbstractKey>[] controllers)
		{
			this.controllers = controllers.ToList();
		}

		public override InputState? GetState(TAbstractKey key)
		{
			return Merge(controllers.Select(x => x.GetState(key)).ToArray());
		}

		public override void Update()
		{
			if (IsChildrenUpdated)
			{
				foreach (var item in controllers)
				{
					item.Update();
				}
			}
		}

		private InputState? Merge(InputState?[] states)
		{
			if (states.Any(x => x == InputState.Hold))
			{
				return InputState.Hold;
			}
			else if (states.Any(x => x == InputState.Push))
			{
				return InputState.Push;
			}
			else if (states.Any(x => x == InputState.Release))
			{
				return InputState.Release;
			}
			else if(states.Any(x => x == InputState.Free))
			{
				return InputState.Free;
			}
			else
			{
				return null;
			}
		}
	}
}
