using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.SceneComponents
{
	public class PushingCoroutineComponent : SceneComponent
	{
		public delegate IEnumerable<Unit> SequenceIterator(Action onNext);

		private IEnumerator<Unit> iterator { get; set; }
		private Action callback { get; set; }
		private Action onNext { get; set; }
		private bool isWaiting { get; set; }

		public PushingCoroutineComponent(SequenceIterator iterator, Action callback = null)
		{
			onNext = () => isWaiting = false;
			this.iterator = iterator(onNext).GetEnumerator();
			this.callback = callback;
			isWaiting = false;
		}

		protected override void OnUpdated()
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
