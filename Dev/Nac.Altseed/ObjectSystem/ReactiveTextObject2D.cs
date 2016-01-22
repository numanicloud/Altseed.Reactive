using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;

namespace Nac.Altseed.ObjectSystem
{
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dテクスチャオブジェクト。
	/// </summary>
	public class ReactiveTextObject2D : TextObject2D, INotifyUpdated
	{
		private Subject<float> onUpdateEvent_ = new Subject<float>();
		private Subject<Unit> onDisposeEvent_ = new Subject<Unit>();

		/// <summary>
		/// 更新されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdateEvent_;
		/// <summary>
		/// 破棄されたときに発行されるイベント。発行されると同時に完了します。
		/// </summary>
		public IObservable<Unit> OnDisposeEvent => onDisposeEvent_;

		protected override void OnUpdate()
		{
			onUpdateEvent_.OnNext(Engine.DeltaTime);
		}

		protected override void OnDispose()
		{
			onDisposeEvent_.OnNext(Unit.Default);
			onUpdateEvent_.OnCompleted();
			onDisposeEvent_.OnCompleted();
		}
	}
}
