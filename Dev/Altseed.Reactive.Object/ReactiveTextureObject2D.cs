using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;

namespace Altseed.Reactive.Object
{
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dテクスチャオブジェクト。
	/// </summary>
	public class ReactiveTextureObject2D : TextureObject2D, INotifyUpdated
	{
		private Subject<Unit> onAddedEvent_ = new Subject<Unit>();
		private Subject<Unit> onRemovedEvent_ = new Subject<Unit>();
		private Subject<Unit> onDisposeEvent_ = new Subject<Unit>();
		private Subject<float> onUpdateEvent_ = new Subject<float>();

		/// <summary>
		/// レイヤーに登録されたときに発行されるイベント。オブジェクトが破棄されたとき完了します。
		/// </summary>
		public IObservable<Unit> OnAddedEvent => onAddedEvent_;
		public IObservable<Unit> OnRemovedEvent => onRemovedEvent_;
		/// <summary>
		/// 更新されたときに発行されるイベント。オブジェクトが破棄されたとき完了します。
		/// 値として Engine.DeltaTime が流れます。
		/// </summary>
		public IObservable<float> OnUpdateEvent => onUpdateEvent_;
		/// <summary>
		/// 破棄されたときに発行されるイベント。オブジェクトが破棄されたとき完了します。
		/// </summary>
		public IObservable<Unit> OnDisposeEvent => onDisposeEvent_;

		protected override void OnAdded()
		{
			onAddedEvent_.OnNext(Unit.Default);
		}

		protected override void OnRemoved()
		{
			onRemovedEvent_.OnNext(Unit.Default);
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
