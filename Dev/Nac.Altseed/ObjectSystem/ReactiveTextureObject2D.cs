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
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dテキストオブジェクト。
	/// </summary>
	public class ReactiveTextureObject2D : TextureObject2D, INotifyUpdated
	{
		private Subject<float> onUpdateEvent_ = new Subject<float>();
		private Subject<Unit> onVanishEvent_ = new Subject<Unit>();

		/// <summary>
		/// 更新されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdateEvent_;
		/// <summary>
		/// 破棄されたときに発行されるイベント。発行されると同時に完了します。
		/// </summary>
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
