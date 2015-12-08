using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;

namespace Nac.Altseed.Reactive.UI
{
	public enum Orientation
	{
		Horizontal, Vertical
	}

	public class ScrollingSelector<TChoice, TAbstractKey> : Layer2D, ISelectableList<TChoice, TAbstractKey>
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
		public Layer2D Layer
		{
			get { return scroll; }
		}

		public bool IsActive
		{
			get { return selector.IsActive; }
			set { selector.IsActive = value; }
		}
		public int SelectedIndex => selector.SelectedIndex;
		public IObservable<TChoice> OnSelectionChanged => selector.OnSelectionChanged;
		public IObservable<TChoice> OnMove => selector.OnMove;
		public IObservable<TChoice> OnDecide => selector.OnDecide;
		public IObservable<TChoice> OnCancel => selector.OnCancel;

		public ScrollingSelector(Controller<TAbstractKey> controller)
		{
			layout = new LinearPanel();
			selector = new Selector<TChoice, TAbstractKey>(controller, layout);
			scroll = new ScrollLayer();

			scroll.AddObject(selector);

			var areaChanged = selector.OnSelectionChanged
				.Select(c => selector.GetItemForChocie(c).Position)
				.Select(p => new RectF(p.X, p.Y, selector.Texture.Size.X, selector.Texture.Size.Y));
			scroll.SubscribeSeeingArea(areaChanged);

			Position = new Vector2DF();
			orientation_ = Orientation.Vertical;
			lineSpan_ = 20;
			lineWidth_ = 200;
			boundLines_ = 1;
			extraLinesOnStarting = 1;
			extraLinesOnEnding = 1;
			ResetBound();
		}

		public void AddChoice(TChoice choice, Object2D obj)
		{
			selector.AddChoice(choice, obj);
			ResetOuterBound();
		}

		public Object2D RemoveChoice(TChoice choice)
		{
			var obj = selector.RemoveChoice(choice);
			ResetOuterBound();
			return obj;
		}


		protected override void OnStart()
		{
			Scene.AddLayer(scroll);
		}


		private void ResetOuterBound()
		{
			Vector2DF widthDirection = new Vector2DF();
			switch(Orientation)
			{
			case Orientation.Horizontal:
				widthDirection = new Vector2DF(0, LineWidth);
				break;
			case Orientation.Vertical:
				widthDirection = new Vector2DF(LineWidth, 0);
				break;
			}

			scroll.Ending = layout.Objects.Count() * layout.ItemSpan + widthDirection;
		}

		private void ResetBound()
		{
			Vector2DF widthDirection = new Vector2DF();
			switch(Orientation)
			{
			case Orientation.Horizontal:
				layout.ItemSpan = new Vector2DF(LineSpan, 0);
				widthDirection = new Vector2DF(0, LineWidth);
				break;
			case Orientation.Vertical:
				layout.ItemSpan = new Vector2DF(0, LineSpan);
				widthDirection = new Vector2DF(LineWidth, 0);
				break;
			}

			var bindStarting = layout.ItemSpan * ExtraLinesOnStarting;
			var bindSize = layout.ItemSpan * BoundLines + widthDirection;
			scroll.CameraSize = layout.ItemSpan * (ExtraLinesOnStarting + BoundLines + ExtraLinesOnEnding) + widthDirection;
			scroll.BindingAreaRange = Helper.GetRectFromVector(bindStarting, bindSize);
		}

		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
		{
			selector.BindKey(next, prev, decide, cancel);
		}

		public void SetDebugCameraUp()
		{
			var viewer = new ScrollBoundViewer(scroll);
			scroll.AddObject(viewer);
		}
	}
}
