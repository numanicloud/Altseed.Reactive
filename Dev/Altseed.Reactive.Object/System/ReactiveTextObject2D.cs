using System;
using System.Reactive;
using System.Reactive.Subjects;
using asd;
using Altseed.Reactive.Helper;
using System.Reactive.Disposables;

namespace Altseed.Reactive.Object
{
	/// <summary>
	/// 特定のタイミングでイベントを発行する2Dテキストオブジェクト。
	/// </summary>
	public class ReactiveTextObject2D : TextObject2D, INotifyUpdated, IDisposer
	{
		private Subject<Unit> onAddedEvent_ = new Subject<Unit>();
		private Subject<Unit> onRemovedEvent_ = new Subject<Unit>();
		private Subject<long> onUpdateEvent_ = new Subject<long>();
		private Subject<Unit> onDisposeEvent_ = new Subject<Unit>();
		private CompositeDisposable disposable = new CompositeDisposable();

		/// <summary>
		/// レイヤーに登録されたときに発行されるイベント。オブジェクトが破棄されたとき完了します。
		/// </summary>
		public IObservable<Unit> OnAddedEvent => onAddedEvent_;
		/// <summary>
		/// レイヤーから削除されたときに発行されるイベント。オブジェクトが破棄されたときに完了します。
		/// </summary>
		public IObservable<Unit> OnRemovedEvent => onRemovedEvent_;
		/// <summary>
		/// 更新されたときに発行されるイベント。破棄されたとき完了します。
		/// 値として何個目のイベントかのインデックスが流れます。
		/// </summary>
		public IObservable<long> OnUpdatedEvent => onUpdateEvent_.Total();
		/// <summary>
		/// 破棄されたときに発行されるイベント。発行されると同時に完了します。
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

		protected override void OnUpdate()
		{
			onUpdateEvent_.OnNext(1);
		}

		protected override void OnDispose()
		{
			onDisposeEvent_.OnNext(Unit.Default);
			onAddedEvent_.OnCompleted();
			onRemovedEvent_.OnCompleted();
			onUpdateEvent_.OnCompleted();
			onDisposeEvent_.OnCompleted();
			disposable.Dispose();
		}

		/// <summary>
		/// このオブジェクトが破棄されるときに一緒に破棄されるインスタンスを設定します
		/// </summary>
		/// <param name="resource">このオブジェクトが破棄されるときに一緒に破棄されるインスタンス。</param>
		public void AddDisposable(IDisposable resource)
		{
			disposable.Add(resource);
		}
	}
}
