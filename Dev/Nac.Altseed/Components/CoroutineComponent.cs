using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Components
{
	public class SequenceSceneComponent : SceneComponent
	{
		public delegate IEnumerable<Unit> SequenceIterator(Action onNext);

		// コールバックで進むのか、毎フレーム進むのかを間違えやすいので、チェックできるようにしたい(型を変える？)

		private IEnumerator<Unit> iterator { get; set; }
		private Action callback { get; set; }
		private Action onNext { get; set; }
		private bool isWaiting { get; set; }

		public SequenceSceneComponent(SequenceIterator iterator, Action callback = null)
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
