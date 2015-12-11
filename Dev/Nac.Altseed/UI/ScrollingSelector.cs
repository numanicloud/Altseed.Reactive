using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.UI
{
	public enum Orientation
	{
		Horizontal, Vertical
	}

	public class ScrollingSelector<TChoice, TAbstractKey> : Layer2D, ISelector<TChoice, TAbstractKey>
	{
		private Orientation orientation_;
		private float lineSpan_, lineWidth_;
		private int boundLines_, extraLinesOnStarting, extraLinesOnEnding;

		private Selector<TChoice, TAbstractKey> selector { get; set; }
		private ScrollLayer scroll { get; set; }
		private LinearPanel layout { get; set; }

		public TextureObject2D Cursor => selector;
		public Vector2DF Position
		{
			get { return scroll.Position; }
			set { scroll.Position = value; }
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

		#region Selectorへの委譲機能
		public bool IsActive
		{
			get { return selector.IsActive; }
			set { selector.IsActive = value; }
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
		public Layer2D Layer => selector.Layer;
		public IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem> ChoiceItems => selector.ChoiceItems;
		public int SelectedIndex => selector.SelectedIndex;
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
			scroll = new ScrollLayer();

			scroll.AddObject(selector);

			var areaChanged = selector.OnSelectionChanged
				.Select(c => Unit.Default)
				.Merge(selector.OnLayoutChanged)
				.Where(u => selector.SelectedIndex != -1)
				.Select(p => Helper.GetRectFromVector(layout.ItemSpan * selector.SelectedIndex, GetSize(1)));
			scroll.SubscribeSeeingArea(areaChanged);
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
		}

		public void SetEasingScrollUp(EasingStart start, EasingEnd end, int time)
		{
			layout.SetEasingBehaviorUp(start, end, time);
			selector.SetEasingBehaviorUp(start, end, time);
			scroll.SetEasingBehaviorUp(start, end, time);
		}

		public void SetDebugCameraUp()
		{
			var viewer = new ScrollBoundViewer(scroll);
			scroll.AddObject(viewer);
		}


		protected override void OnStart()
		{
			Scene.AddLayer(scroll);
		}


		private void ResetOuterBound()
		{
			scroll.Ending = GetSize(layout.Items.Count());
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
			scroll.CameraSize = GetSize(ExtraLinesOnStarting + BoundLines + ExtraLinesOnEnding);
			scroll.BindingAreaRange = Helper.GetRectFromVector(bindStarting, bindSize);
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
