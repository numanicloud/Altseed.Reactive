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
	public class ReactiveTextObject2D : TextObject2D, INotifyUpdated
	{
		private Subject<float> onUpdateEvent_ = new Subject<float>();
		private Subject<Unit> onVanishEvent_ = new Subject<Unit>();

		public IObservable<float> OnUpdateEvent => onUpdateEvent_;
		public IObservable<Unit> OnVanishEvent => onVanishEvent_;

		protected override void OnUpdate()
		{
			onUpdateEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnVanish()
		{
			onVanishEvent_.OnNext(Unit.Default);
			onUpdateEvent_.OnCompleted();
			onVanishEvent_.OnCompleted();
		}
	}
}
