using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public class LinearPanel : Layouter
    {
		public class ItemInfo
		{
			private LinearPanel owner { get; set; }
			public Object2D Object { get; private set; }
			public IDisposable Cancellation { get; private set; }
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
				Cancellation = owner.SetItemPosition(Object, goal)
					.Subscribe(p => Object.Position = p);
			}
		}

        private ObservableCollection<ItemInfo> items_ { get; set; }
        private List<IDisposable> cancellations { get; set; }
        private Vector2DF startingOffset, itemSpan;
		private Subject<Unit> onLayoutChanged_ = new Subject<Unit>();

        protected override IEnumerable<Object2D> ObjectsInternal => items_.Select(x => x.Object);

		public IObservable<Unit> OnLayoutChanged => onLayoutChanged_;
		public INotifyCollectionChanged ObjectsNotification => items_;
        public IEnumerable<ItemInfo> Items => items_;
        public Vector2DF StartingOffset
        {
            get { return startingOffset; }
            set
            {
                startingOffset = value;
                ResetPosition();
            }
        }
        public Vector2DF ItemSpan
        {
            get { return itemSpan; }
            set
            {
                itemSpan = value;
                ResetPosition();
            }
        }
        public Func<Object2D, Vector2DF, IObservable<Vector2DF>> SetItemPosition { get; set; }

        public LinearPanel()
        {
            items_ = new ObservableCollection<ItemInfo>();
            cancellations = new List<IDisposable>();
			SetItemPosition = (o, v) => Observable.Return(v);
        }

		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			SetItemPosition = (o, v) => UpdateManager.Instance.FrameUpdate
				.Select(t => o.Position)
				.EasingVector2DF(v, start, end, time);
		}

        public override void AddItem(Object2D item)
        {
            item.Position = GetPosition(items_.Count);
            AddChild(item, ChildMode.Position);
            items_.Add(new ItemInfo(this, item));
            cancellations.Add(null);
			onLayoutChanged_.OnNext(Unit.Default);
        }

        public override void InsertItem(int index, Object2D item)
        {
            item.Position = GetPosition(index);
            AddChild(item, ChildMode.Position);
            items_.Insert(index, new ItemInfo(this, item));
            cancellations.Insert(index, null);

            for(int i = index + 1; i < items_.Count; i++)
            {
				items_[i].StartAnimation(GetPosition(i));
            }

			onLayoutChanged_.OnNext(Unit.Default);
		}

        public override void RemoveItem(Object2D item)
        {
            var index = items_.IndexOf(x => x.Object == item);
            RemoveChild(item);
            items_.RemoveAt(index);
            cancellations.RemoveAt(index);
            
            for(int i = index; i < items_.Count; i++)
			{
				items_[i].StartAnimation(GetPosition(i));
            }

			onLayoutChanged_.OnNext(Unit.Default);
		}

        public override void ClearItem()
        {
            foreach(var item in items_)
            {
                RemoveChild(item.Object);
            }
            items_.Clear();
            cancellations.Clear();
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
