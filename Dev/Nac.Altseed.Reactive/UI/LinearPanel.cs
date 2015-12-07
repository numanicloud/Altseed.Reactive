using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public class LinearPanel : Layouter
    {
        private List<Object2D> objects_ { get; set; }
        private CompositeDisposable cancellationOfRemovingAction { get; set; }
        private Vector2DF startingOffset, itemSpan;

        protected override IEnumerable<Object2D> ObjectsInternal => objects_;

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
            objects_ = new List<Object2D>();
            cancellationOfRemovingAction = new CompositeDisposable();
            SetItemPosition = (o, v) => o.SetEasing(v, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 20);
        }

        public override void AddItem(Object2D child)
        {
            child.Position = StartingOffset + objects_.Count * ItemSpan;
            AddChild(child, ChildMode.Position);
            objects_.Add(child);
        }

        public override void RemoveItem(Object2D child)
        {
            var index = objects_.IndexOf(child);

            cancellationOfRemovingAction.Clear();
            for(int i = index + 1; i < objects_.Count; i++)
            {
                SetItemPosition(objects_[i], StartingOffset + (i - 1) * ItemSpan);
            }

            objects_.Remove(child);
        }

        private void ResetPosition()
        {
            cancellationOfRemovingAction.Clear();
            for(int i = 0; i < objects_.Count; i++)
            {
                objects_[i].Position = StartingOffset + ItemSpan * i;
            }
        }
    }
}
