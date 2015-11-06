using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Object2DComponents
{
	public class CoroutineComponent : Object2DComponent
	{
		private IEnumerator<Unit> iterator { get; set; }
		private Action callback { get; set; }
		private bool isWaiting { get; set; }

		public CoroutineComponent(IEnumerable<Unit> iterator, Action callback = null)
		{
			this.iterator = iterator.GetEnumerator();
			this.callback = callback;
			isWaiting = false;
		}

		protected override void OnUpdate()
		{
			if(!isWaiting)
			{
				var result = iterator.MoveNext();
				if(!result)
				{
					callback?.Invoke();
					Vanish();
				}
			}
		}
	}
}
