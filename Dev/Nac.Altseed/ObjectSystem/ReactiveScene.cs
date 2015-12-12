using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.ObjectSystem
{
	public class ReactiveScene : Scene, INotifyUpdated
	{
		private Subject<float> onUpdatedEvent_ = new Subject<float>();
		private Subject<Unit> onUpdatedForTheFirstTimeEvent_ = new Subject<Unit>();

		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;
		public IObservable<Unit> OnUpdatedForTheFirstTimeEvent => onUpdatedForTheFirstTimeEvent_;

		protected override void OnUpdateForTheFirstTime()
		{
			onUpdatedForTheFirstTimeEvent_.OnNext(Unit.Default);
			onUpdatedForTheFirstTimeEvent_.OnCompleted();
		}

		protected override void OnUpdated()
		{
			onUpdatedEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnChanging()
		{
			onUpdatedEvent_.OnCompleted();
		}
	}
}
