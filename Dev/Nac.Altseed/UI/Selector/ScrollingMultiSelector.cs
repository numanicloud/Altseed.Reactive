using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	public class ScrollingMultiSelector<TChoice, TAbstractKey>
		: ReactiveLayer2D, IScrollingSelector, IMultiSelector<TChoice, TAbstractKey>, ISelector<TChoice, TAbstractKey>
	{
		private ScrollingSelectorBase<TChoice, TAbstractKey> scrollingSelector;
		private MultiSelector<TChoice, TAbstractKey> multiSelector;

		#region IScrollingSelector実装
		public TextureObject2D Cursor
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).Cursor;
			}
		}

		public ReactiveLayer2D ScrollLayer
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).ScrollLayer;
			}
		}

		public Vector2DF Position
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).Position;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).Position = value;
			}
		}

		public Orientation Orientation
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).Orientation;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).Orientation = value;
			}
		}

		public float LineSpan
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).LineSpan;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).LineSpan = value;
			}
		}

		public float LineWidth
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).LineWidth;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).LineWidth = value;
			}
		}

		public int BoundLines
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).BoundLines;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).BoundLines = value;
			}
		}

		public int ExtraLinesOnStarting
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).ExtraLinesOnStarting;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).ExtraLinesOnStarting = value;
			}
		}

		public int ExtraLinesOnEnding
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).ExtraLinesOnEnding;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).ExtraLinesOnEnding = value;
			}
		}

		public Vector2DF LayoutStarting
		{
			get
			{
				return ((IScrollingSelector)scrollingSelector).LayoutStarting;
			}

			set
			{
				((IScrollingSelector)scrollingSelector).LayoutStarting = value;
			}
		} 

		public void SetEasingScrollUp(EasingStart start, EasingEnd end, int time)
		{
			((IScrollingSelector)scrollingSelector).SetEasingScrollUp(start, end, time);
		}

		public void SetDebugCameraUp()
		{
			((IScrollingSelector)scrollingSelector).SetDebugCameraUp();
		}
		#endregion

		#region IMultiSelector実装
		public IObservable<TChoice> OnAdd
		{
			get
			{
				return ((IMultiSelector<TChoice, TAbstractKey>)multiSelector).OnAdd;
			}
		}

		public IObservable<TChoice> OnRemove
		{
			get
			{
				return ((IMultiSelector<TChoice, TAbstractKey>)multiSelector).OnRemove;
			}
		}

		public IObservable<TChoice> OnDecideForSingle
		{
			get
			{
				return ((IMultiSelector<TChoice, TAbstractKey>)multiSelector).OnDecideForSingle;
			}
		}

		public IObservable<IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem>> OnDecideForMulti
		{
			get
			{
				return ((IMultiSelector<TChoice, TAbstractKey>)multiSelector).OnDecideForMulti;
			}
		}

		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel, TAbstractKey multi)
		{
			((IMultiSelector<TChoice, TAbstractKey>)multiSelector).BindKey(next, prev, decide, cancel, multi);
		}

		public void AddSelectedIndex()
		{
			((IMultiSelector<TChoice, TAbstractKey>)multiSelector).AddSelectedIndex();
		}

		public void RemoveSelectedIndex()
		{
			((IMultiSelector<TChoice, TAbstractKey>)multiSelector).RemoveSelectedIndex();
		}

		public void Refresh()
		{
			((IMultiSelector<TChoice, TAbstractKey>)multiSelector).Refresh();
		}
		#endregion

		#region ISelector実装
		public bool IsActive
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).IsActive;
			}

			set
			{
				((ISelector<TChoice, TAbstractKey>)multiSelector).IsActive = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).SelectedIndex;
			}
		}

		public IObservable<TChoice> OnSelectionChanged
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).OnSelectionChanged;
			}
		}

		public IObservable<TChoice> OnMove
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).OnMove;
			}
		}

		public IObservable<TChoice> OnDecide
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).OnDecide;
			}
		}

		public IObservable<TChoice> OnCancel
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)multiSelector).OnCancel;
			}
		}

		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
		{
			((ISelector<TChoice, TAbstractKey>)multiSelector).BindKey(next, prev, decide, cancel);
		}

		public void AddChoice(TChoice choice, Object2D obj)
		{
			((ISelector<TChoice, TAbstractKey>)multiSelector).AddChoice(choice, obj);
		}

		public Object2D RemoveChoice(TChoice choice)
		{
			return ((ISelector<TChoice, TAbstractKey>)multiSelector).RemoveChoice(choice);
		}

		public void InsertChoice(int index, TChoice choice, Object2D obj)
		{
			((ISelector<TChoice, TAbstractKey>)multiSelector).InsertChoice(index, choice, obj);
		}

		public void ClearChoice()
		{
			((ISelector<TChoice, TAbstractKey>)multiSelector).ClearChoice();
		}
		#endregion

		public bool IsControllerUpdated
		{
			get { return multiSelector.IsControllerUpdated; }
			set { multiSelector.IsControllerUpdated = value; }
		}
		public bool Loop
		{
			get { return multiSelector.Loop; }
			set { multiSelector.Loop = value; }
		}
		public Vector2DF CursorOffset
		{
			get { return scrollingSelector.CursorOffset; }
			set { scrollingSelector.CursorOffset = value; }
		}
		public IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem> ChoiceItems => multiSelector.ChoiceItems;

		public ScrollingMultiSelector(Controller<TAbstractKey> controller, Func<Object2D> createCursor)
		{
			var layout = new LinearPanel();
			multiSelector = new MultiSelector<TChoice, TAbstractKey>(controller, layout, createCursor);
			scrollingSelector = new ScrollingSelectorBase<TChoice, TAbstractKey>(multiSelector, layout);
		}

		protected override void OnAdded()
		{
			base.OnAdded();
			Scene.AddLayer(scrollingSelector.ScrollLayer);
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			Scene.RemoveLayer(scrollingSelector.ScrollLayer);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			scrollingSelector.ScrollLayer.Dispose();
		}
	}
}
