using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public class LinearPanel : Layouter
    {
        private ObservableCollection<Object2D> objects_ { get; set; }
        private List<IDisposable> cancellations { get; set; }
        private Vector2DF startingOffset, itemSpan;

        protected override IEnumerable<Object2D> ObjectsInternal => objects_;

		public INotifyCollectionChanged ObjectsNotification => objects_;
        public IEnumerable<Object2D> Objects => objects_;
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
        public Func<Object2D, Vector2DF, Cancelable> SetItemPosition { get; set; }

        public LinearPanel()
        {
            objects_ = new ObservableCollection<Object2D>();
            cancellations = new List<IDisposable>();
            SetItemPosition = (o, v) => o.SetEasing(v, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 20);
        }

        public override void AddItem(Object2D item)
        {
            item.Position = GetPosition(objects_.Count);
            AddChild(item, ChildMode.Position);
            objects_.Add(item);
            cancellations.Add(null);
        }

        public override void InsertItem(int index, Object2D item)
        {
            item.Position = GetPosition(index);
            AddChild(item, ChildMode.Position);
            objects_.Insert(index, item);
            cancellations.Insert(index, null);

            for(int i = index + 1; i < objects_.Count; i++)
            {
                cancellations[i]?.Dispose();
                cancellations[i] = SetItemPosition(objects_[i], GetPosition(i));
            }
        }

        public override void RemoveItem(Object2D item)
        {
            var index = objects_.IndexOf(item);
            RemoveChild(item);
            objects_.Remove(item);
            cancellations.RemoveAt(index);
            
            for(int i = index; i < objects_.Count; i++)
            {
                cancellations[i]?.Dispose();
                cancellations[i] = SetItemPosition(objects_[i], GetPosition(i));
            }
        }

        public override void ClearItem()
        {
            foreach(var item in objects_)
            {
                RemoveChild(item);
            }
            objects_.Clear();
            cancellations.Clear();
        }

        private void ResetPosition()
        {
            for(int i = 0; i < objects_.Count; i++)
            {
                cancellations[i]?.Dispose();
                objects_[i].Position = GetPosition(i);
            }
        }

        private Vector2DF GetPosition(int index)
        {
            return StartingOffset + index * ItemSpan;
        }
    }
}
