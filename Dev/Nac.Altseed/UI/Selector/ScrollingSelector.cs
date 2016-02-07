using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
    public enum Orientation
    {
        Horizontal, Vertical
    }

    public class ScrollingSelector<TChoice, TAbstractKey>
		: ReactiveLayer2D, ISelector<TChoice, TAbstractKey>, IScrollingSelector
    {
        private Orientation orientation_;
        private float lineSpan_, lineWidth_;
        private int boundLines_, extraLinesOnStarting, extraLinesOnEnding;

        private Selector<TChoice, TAbstractKey> selector { get; set; }
		private ScrollLayer scrollLayer_ { get; set; }
        private LinearPanel layout { get; set; }

        public TextureObject2D Cursor => selector.Cursor;
		public ReactiveLayer2D ScrollLayer => scrollLayer_;
		public Vector2DF Position
        {
            get { return scrollLayer_.Position; }
            set { scrollLayer_.Position = value; }
        }
        public Orientation Orientation
        {
            get { return orientation_; }
            set
            {
                orientation_ = value;
                ResetBound();
            }
        }
        public float LineSpan
        {
            get { return lineSpan_; }
            set
            {
                lineSpan_ = value;
                ResetBound();
            }
        }
        public float LineWidth
        {
            get { return lineWidth_; }
            set
            {
                lineWidth_ = value;
                ResetBound();
            }
        }
        public int BoundLines
        {
            get { return boundLines_; }
            set
            {
                boundLines_ = value;
                ResetBound();
            }
        }
        public int ExtraLinesOnStarting
        {
            get { return extraLinesOnStarting; }
            set
            {
                extraLinesOnStarting = value;
                ResetBound();
            }
        }
        public int ExtraLinesOnEnding
        {
            get { return extraLinesOnEnding; }
            set
            {
                extraLinesOnEnding = value;
                ResetBound();
            }
        }

        public Vector2DF LayoutStarting
        {
            get { return layout.StartingOffset; }
            set { layout.StartingOffset = value; }
        }

		public bool IsControllerUpdated
		{
			get { return selector.IsControllerUpdated; }
			set { selector.IsControllerUpdated = value; }
		}
		public bool Loop
		{
			get { return selector.Loop; }
			set { selector.Loop = value; }
		}
		public Vector2DF CursorOffset
		{
			get { return selector.CursorOffset; }
			set
			{
				selector.CursorOffset = value;
				ResetBound();
				ResetOuterBound();
			}
		}

		#region Selectorへの委譲機能
		public bool IsActive
        {
            get { return selector.IsActive; }
            set { selector.IsActive = value; }
        }
        public IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem> ChoiceItems => selector.ChoiceItems;
        public int SelectedIndex => selector.SelectedIndex;
		public TChoice SelectedChoice => selector.SelectedChoice;
        public IObservable<TChoice> OnSelectionChanged => selector.OnSelectionChanged;
        public IObservable<TChoice> OnMove => selector.OnMove;
        public IObservable<TChoice> OnDecide => selector.OnDecide;
        public IObservable<TChoice> OnCancel => selector.OnCancel;

        public void AddChoice(TChoice choice, Object2D obj)
        {
            selector.AddChoice(choice, obj);
        }

        public Object2D RemoveChoice(TChoice choice)
        {
            return selector.RemoveChoice(choice);
        }

        public Object2D GetItemForChoice(TChoice choice)
        {
            return selector.GetItemForChocie(choice);
        }

        public void InsertChoice(int index, TChoice choice, Object2D obj)
        {
            selector.InsertChoice(index, choice, obj);
        }

        public void ClearChoice()
        {
            selector.ClearChoice();
        }

        public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
        {
            selector.BindKey(next, prev, decide, cancel);
        }
        #endregion

        public ScrollingSelector(Controller<TAbstractKey> controller)
        {
            layout = new LinearPanel();
            selector = new Selector<TChoice, TAbstractKey>(controller, layout);
            scrollLayer_ = new ScrollLayer();

			Name = "Selector";
			scrollLayer_.Name = "Scroll";

            var areaChanged = selector.OnSelectionChanged
                .Select(c => Unit.Default)
                .Merge(selector.OnLayoutChanged)
                .Where(u => selector.SelectedIndex != -1)
                .Select(p => GeometoryHelper.GetRectFromVector(layout.ItemSpan * selector.SelectedIndex + selector.CursorOffset, GetSize(1)));
            scrollLayer_.SubscribeSeeingArea(areaChanged);
            layout.OnLayoutChanged.Subscribe(u => ResetOuterBound());

            Position = new Vector2DF();
            orientation_ = Orientation.Vertical;
            lineSpan_ = 20;
            lineWidth_ = 200;
            boundLines_ = 1;
            extraLinesOnStarting = 1;
            extraLinesOnEnding = 1;
            ResetOuterBound();
            ResetBound();

			scrollLayer_.AddObject(selector);
        }

        public void SetEasingScrollUp(EasingStart start, EasingEnd end, int time)
        {
            layout.SetEasingBehaviorUp(start, end, time);
            selector.SetEasingBehaviorUp(start, end, time);
            scrollLayer_.SetEasingBehaviorUp(start, end, time);
        }

        public void SetDebugCameraUp()
        {
            var viewer = new ScrollBoundViewer(scrollLayer_);
            scrollLayer_.AddObject(viewer);
        }

		protected override void OnAdded()
		{
			base.OnAdded();
			Scene.AddLayer(scrollLayer_);
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			Scene.RemoveLayer(scrollLayer_);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			scrollLayer_.Dispose();
		}


        private void ResetOuterBound()
        {
			scrollLayer_.Starting = LayoutStarting + selector.CursorOffset;
            scrollLayer_.Ending = GetSize(layout.Items.Count()) + LayoutStarting + selector.CursorOffset;
        }

        private void ResetBound()
        {
            switch(Orientation)
            {
            case Orientation.Horizontal:
                layout.ItemSpan = new Vector2DF(LineSpan, 0);
                break;
            case Orientation.Vertical:
                layout.ItemSpan = new Vector2DF(0, LineSpan);
                break;
            }

            var bindStarting = layout.ItemSpan * ExtraLinesOnStarting;
            var bindSize = GetSize(BoundLines);
            scrollLayer_.CameraSize = GetSize(ExtraLinesOnStarting + BoundLines + ExtraLinesOnEnding);
            scrollLayer_.BindingAreaRange = GeometoryHelper.GetRectFromVector(bindStarting, bindSize);
        }

        private Vector2DF GetSize(int lines)
        {
            Vector2DF size = new Vector2DF();
            switch(Orientation)
            {
            case Orientation.Horizontal:
                size.X = LineSpan * lines;
                size.Y = LineWidth;
                break;
            case Orientation.Vertical:
                size.X = LineWidth;
                size.Y = LineSpan * lines;
                break;
            }
            return size;
        }
    }
}
