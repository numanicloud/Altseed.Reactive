using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Reactive.Input
{
	public enum InputState
	{
		Push, Release, Hold, Free
	}

	public abstract class Controller<TAbstractKey>
	{
		public abstract	IEnumerable<TAbstractKey> Keys { get; }

		public abstract InputState? GetState(TAbstractKey key);

		public virtual void Update()
		{
		}
	}
}
