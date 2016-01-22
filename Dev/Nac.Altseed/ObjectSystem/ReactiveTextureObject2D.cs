using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;

namespace Nac.Altseed.ObjectSystem
{
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dテキストオブジェクト。
	/// </summary>
	public class ReactiveTextureObject2D : TextureObject2D, INotifyUpdated
	{
		private Subject<Unit> onAddedEvent_ = new Subject<Unit>();
		private Subject<Unit> onDisposeEvent_ = new Subject<Unit>();
		private Subject<float> onUpdateEvent_ = new Subject<float>();

		/// <summary>
		/// 破棄されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<Unit> OnDisposeEvent => onDisposeEvent_;
		/// <summary>
		/// レイヤーに登録されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<Unit> OnAddedEvent => onAddedEvent_;
		/// <summary>
		/// 更新されたときに発行されるイベント。破棄されたとき完了します。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdateEvent_;

		protected override void OnAdded()
		{
			onAddedEvent_.OnNext(Unit.Default);
		}

		protected override void OnDispose()
		{
			onDisposeEvent_.OnNext(Unit.Default);
			onUpdateEvent_.OnCompleted();
			onDisposeEvent_.OnCompleted();
			onAddedEvent_.OnCompleted();
		}

		protected override void OnUpdate()
		{
			onUpdateEvent_.OnNext(Engine.DeltaTime);
		}
	}
}
