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
	public class ReactiveLayer2D : Layer2D, INotifyUpdated
	{
		private Subject<float> onUpdatedEvent_ = new Subject<float>();
		private Subject<Unit> onVanisEvent_ = new Subject<Unit>();

		public IObservable<float> OnUpdateEvent => onUpdatedEvent_;
		public IObservable<Unit> OnVanishEvent => onVanisEvent_;

		protected override void OnUpdated()
		{
			onUpdatedEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnVanish()
		{
			onVanisEvent_.OnNext(Unit.Default);
			onUpdatedEvent_.OnCompleted();
			onVanisEvent_.OnCompleted();
		}
	}
}
