using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using asd;
using Altseed.Reactive.Helper;
using Altseed.Reactive.Easings;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// オブジェクトを一直線に配置するレイアウト クラス。
	/// </summary>
	/// <remarks>このクラスにオブジェクトを登録すると、同時にこのオブジェクトの子オブジェクトとなります。</remarks>
	public class LinearPanel : Layouter
    {
		public class ItemInfo
		{
			private LinearPanel owner { get; set; }

			/// <summary>
			/// レイアウトされているオブジェクトを取得します。
			/// </summary>
			public Object2D Object { get; private set; }

			/// <summary>
			/// Disposeすることでこのオブジェクトのアニメーションを停止できます。
			/// </summary>
			public IDisposable Cancellation { get; private set; }

			/// <summary>
			/// このオブジェクトがレイアウトされている位置を取得します。
			/// アニメーション中でも正しい位置を取得できます。
			/// </summary>
			public Vector2DF LayoutedPosition { get; private set; }

			public ItemInfo(LinearPanel owner, Object2D obj)
			{
				this.owner = owner;
				Object = obj;
				LayoutedPosition = obj.Position;
			}

			public void StopAnimation()
			{
				Cancellation?.Dispose();
			}

			public void StartAnimation(Vector2DF goal)
			{
				LayoutedPosition = goal;
				Cancellation?.Dispose();
				Cancellation = owner.GetNewItemPosition(Object, goal)
					.Where(p => Object.IsAlive)
					.Subscribe(p => Object.Position = p);
			}
		}

		private ObservableCollection<ItemInfo> items_;
        private Vector2DF startingOffset_, itemSpan_;
		private readonly Subject<Unit> onLayoutChanged_ = new Subject<Unit>();

		private List<IDisposable> Cancellations { get; set; }
		protected override IEnumerable<Object2D> ObjectsInternal => items_.Select(x => x.Object);

		/// <summary>
		/// レイアウトされる2Dオブジェクトの配置が更新されたときに通知するイベントを取得します。
		/// </summary>
		public IObservable<Unit> OnLayoutChanged => onLayoutChanged_;

		/// <summary>
		/// レイアウトされる2Dオブジェクトのコレクションが変更されたときに通知するインターフェースを取得します。
		/// </summary>
		public INotifyCollectionChanged ObjectsNotification => items_;

		/// <summary>
		/// レイアウトされる2Dオブジェクトのコレクションを取得します。
		/// </summary>
        public IEnumerable<ItemInfo> Items => items_;

		/// <summary>
		/// レイアウトの開始位置を取得または設定します。
		/// </summary>
        public Vector2DF StartingOffset
        {
            get { return startingOffset_; }
            set
            {
                startingOffset_ = value;
                ResetPosition();
            }
        }
		/// <summary>
		/// 要素どうしの間の間隔を取得または設定します。
		/// </summary>
        public Vector2DF ItemSpan
        {
            get { return itemSpan_; }
            set
            {
                itemSpan_ = value;
                ResetPosition();
            }
        }

		/// <summary>
		/// 要素の配置が変わるときのアニメーションを <see cref="IObservable{Vector2DF}"/> の
		/// インスタンスとして取得するデリゲートを取得または設定します。
		/// </summary>
        public Func<Object2D, Vector2DF, IObservable<Vector2DF>> GetNewItemPosition { get; set; }

		/// <summary>
		/// LinearPanelクラスを初期化します。
		/// </summary>
        public LinearPanel()
        {
            items_ = new ObservableCollection<ItemInfo>();
            Cancellations = new List<IDisposable>();
			GetNewItemPosition = (o, v) => Observable.Return(v);
			IsDrawn = false;
        }

		/// <summary>
		/// 要素の移動を滑らかなアニメーションで表現するように準備します。
		/// </summary>
		/// <param name="start">アニメーションの開始速度。</param>
		/// <param name="end">アニメーションの終了速度。</param>
		/// <param name="time">アニメーションにかけるフレーム数。</param>
		public void SetEasingBehaviorUp(IEasing easing, int time)
		{
			GetNewItemPosition = (o, v) =>
			{
				var initial = o.Position;
				return OnUpdatedEvent.TakeWhile(f => o.IsAlive)
					.Select((x, i) => easing.GetGeneralValue(i, time, 0, 1))
					.Select(t => o.Position = initial * (1 - t) + v * t);
			};
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウトの末尾に配置します。
		/// </summary>
		/// <param name="item">配置する2Dオブジェクト。</param>
        public override void AddItem(Object2D item)
        {
            item.Position = GetPosition(items_.Count);
            AddChild(item, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal, ChildTransformingMode.Position);
            items_.Add(new ItemInfo(this, item));
            Cancellations.Add(null);
			onLayoutChanged_.OnNext(Unit.Default);
        }

		public void AddItem(DrawnObject2D item, ChildDrawingMode drawingMode)
		{
			item.Position = GetPosition(items_.Count);
			AddDrawnChild(item,
				ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal,
				ChildTransformingMode.Position,
				drawingMode);
			items_.Add(new ItemInfo(this, item));
			Cancellations.Add(null);
			onLayoutChanged_.OnNext(Unit.Default);
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウト上に挿入します。
		/// </summary>
		/// <param name="index">挿入する位置のインデックス。</param>
		/// <param name="item">挿入する2Dオブジェクト。</param>
        public override void InsertItem(int index, Object2D item)
        {
            item.Position = GetPosition(index);
            AddChild(item, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal, ChildTransformingMode.Position);
            items_.Insert(index, new ItemInfo(this, item));
            Cancellations.Insert(index, null);

            for(int i = index + 1; i < items_.Count; i++)
            {
				items_[i].StartAnimation(GetPosition(i));
            }

			onLayoutChanged_.OnNext(Unit.Default);
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウトから取り除きます。
		/// </summary>
		/// <param name="item">取り除く2Dオブジェクト。</param>
        public override void RemoveItem(Object2D item)
        {
            var index = items_.IndexOf(x => x.Object == item);
            RemoveChild(item);
            items_.RemoveAt(index);
            Cancellations.RemoveAt(index);
            
            for(int i = index; i < items_.Count; i++)
			{
				items_[i].StartAnimation(GetPosition(i));
            }

			onLayoutChanged_.OnNext(Unit.Default);
		}

		/// <summary>
		/// このレイアウトから全ての要素を取り除きます。
		/// </summary>
        public override void ClearItem()
        {
            foreach(var item in items_)
            {
                RemoveChild(item.Object);
            }
            items_.Clear();
            Cancellations.Clear();
			onLayoutChanged_.OnNext(Unit.Default);
		}

        private void ResetPosition()
        {
            for(int i = 0; i < items_.Count; i++)
            {
				items_[i].StartAnimation(GetPosition(i));
			}
			onLayoutChanged_.OnNext(Unit.Default);
		}

        private Vector2DF GetPosition(int index)
        {
            return StartingOffset + index * ItemSpan;
        }
    }
}
