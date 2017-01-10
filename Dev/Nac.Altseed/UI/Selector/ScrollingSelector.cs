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
	    private ScrollLayer scrollLayer_;
        private Orientation orientation_;
        private float lineSpan_, lineWidth_;
        private int boundLines_, extraLinesOnStarting_, extraLinesOnEnding_;

        private Selector<TChoice, TAbstractKey> Selector { get; set; }
        private LinearPanel layout { get; set; }

        public TextureObject2D Cursor => Selector.Cursor;
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
            get { return extraLinesOnStarting_; }
            set
            {
                extraLinesOnStarting_ = value;
                ResetBound();
            }
        }
        public int ExtraLinesOnEnding
        {
            get { return extraLinesOnEnding_; }
            set
            {
                extraLinesOnEnding_ = value;
                ResetBound();
            }
        }
		/// <summary>
		/// 選択肢の端まで達していてもスクロールするかどうかの真偽値を取得または設定します。
		/// </summary>
	    public bool ScrollIntoVoid { get; set; }

        public Vector2DF LayoutStarting
        {
            get { return layout.StartingOffset; }
            set { layout.StartingOffset = value; }
        }

		public bool IsControllerUpdated
		{
			get { return Selector.IsControllerUpdated; }
			set { Selector.IsControllerUpdated = value; }
		}
		public bool Loop
		{
			get { return Selector.Loop; }
			set { Selector.Loop = value; }
		}
		public Vector2DF CursorOffset
		{
			get { return Selector.CursorOffset; }
			set
			{
				Selector.CursorOffset = value;
				ResetBound();
				ResetOuterBound();
			}
		}

		#region Selectorへの委譲機能

	    /// <summary>
	    /// この選択UIが操作中(アクティブ)であるかどうかの真偽値を取得または設定します。
	    /// </summary>
	    public bool IsActive
        {
            get { return Selector.IsActive; }
            set { Selector.IsActive = value; }
        }
        public IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem> ChoiceItems => Selector.ChoiceItems;

	    /// <summary>
	    /// 選択中の選択肢を取得します。
	    /// </summary>
	    public TChoice SelectedChoice => Selector.SelectedChoice;

	    /// <summary>
	    /// 選択が移動したことを通知するイベントを取得または設定します。
	    /// </summary>
	    public IObservable<TChoice> OnSelectionChanged => Selector.OnSelectionChanged;

	    /// <summary>
	    /// ユーザーの入力によって選択が移動したことを通知するイベントを取得または設定します。
	    /// </summary>
	    public IObservable<TChoice> OnMove => Selector.OnMove;

	    /// <summary>
	    /// 選択が確定されたことを通知するイベントを取得または設定します。
	    /// </summary>
	    public IObservable<TChoice> OnDecide => Selector.OnDecide;

	    /// <summary>
	    /// 選択がキャンセルされたことを通知するイベントを取得または設定します。
	    /// </summary>
	    public IObservable<TChoice> OnCancel => Selector.OnCancel;

	    /// <summary>
	    /// この選択UIのアクティブ状態が変化した時に通知するイベントを取得します。
	    /// </summary>
	    public IObservable<bool> OnActivationStateChanged => Selector.OnActivationStateChanged;

	    /// <summary>
	    /// 選択中の選択肢のインデックスを取得または設定します。
	    /// </summary>
	    public int SelectedIndex
		{
			get { return Selector.SelectedIndex; }
		}

	    public Object2D SelectedItem => Selector.SelectedItem;

		public IEnumerable<TChoice> AvailableChoices
		{
			get
			{
				return ((ISelector<TChoice, TAbstractKey>)Selector).AvailableChoices;
			}
		}

		/// <summary>
		/// UIに選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="item">追加する選択肢を表示する2Dオブジェクト。</param>
		public void AddChoice(TChoice choice, Object2D obj)
        {
            Selector.AddChoice(choice, obj);
        }

	    /// <summary>
	    /// UIから選択肢を削除します。
	    /// </summary>
	    /// <param name="choice">削除する選択肢。</param>
	    public Object2D RemoveChoice(TChoice choice)
        {
            return Selector.RemoveChoice(choice);
        }

        public Object2D GetItemForChoice(TChoice choice)
        {
            return Selector.GetItemForChocie(choice);
        }

	    public void ChangeSelection(int index)
	    {
		    Selector.ChangeSelection(index);
	    }

		/// <summary>
		/// UIに選択肢を挿入します。
		/// </summary>
		/// <param name="index">挿入する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="obj">挿入する選択肢を表示する2Dオブジェクト。</param>
		public void InsertChoice(int index, TChoice choice, Object2D obj)
        {
            Selector.InsertChoice(index, choice, obj);
        }

	    /// <summary>
	    /// UIからすべての選択肢を削除します。
	    /// </summary>
	    public void ClearChoice()
        {
            Selector.ClearChoice();
        }

        public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
        {
            Selector.BindKey(next, prev, decide, cancel);
        }
        #endregion

        public ScrollingSelector(Controller<TAbstractKey> controller)
        {
            layout = new LinearPanel();
            Selector = new Selector<TChoice, TAbstractKey>(controller, layout);
            scrollLayer_ = new ScrollLayer();

			Name = "Selector";
			scrollLayer_.Name = "Scroll";

            var areaChanged = Selector.OnSelectionChanged
                .Select(c => Unit.Default)
                .Merge(Selector.OnLayoutChanged)
                .Where(u => Selector.SelectedIndex != -1)
				.Select(u => Selector.ChoiceItems[Selector.SelectedIndex].Item)
				.Select(o => (o as TextureObject2D)?.CenterPosition ?? new Vector2DF(0, 0))
				.Select(c => new RectF(layout.ItemSpan * Selector.SelectedIndex - c, GetSize(1)));
            scrollLayer_.SubscribeSeeingArea(areaChanged);
            Selector.OnLayoutChanged.Subscribe(u => ResetOuterBound());

            Position = new Vector2DF();
            orientation_ = Orientation.Vertical;
            lineSpan_ = 20;
            lineWidth_ = 200;
            boundLines_ = 1;
            extraLinesOnStarting_ = 0;
            extraLinesOnEnding_ = 0;
            ResetOuterBound();
            ResetBound();

			scrollLayer_.AddObject(Selector);
        }

        public void SetEasingScrollUp(EasingStart start, EasingEnd end, int time)
        {
            layout.SetEasingBehaviorUp(start, end, time);
            Selector.SetEasingBehaviorUp(start, end, time);
            scrollLayer_.SetEasingBehaviorUp(start, end, time);
        }

        public void SetDebugCameraUp()
        {
            var viewer = new ScrollBoundViewer(scrollLayer_);
            scrollLayer_.AddObject(viewer);
        }

	    /// <summary>
	    /// オーバーライドして、このレイヤーがシーンに登録されたときの処理を記述できる。
	    /// </summary>
	    protected override void OnAdded()
		{
			base.OnAdded();
			Scene.AddLayer(scrollLayer_);
		}

	    /// <summary>
	    /// オーバーライドして、このレイヤーがシーンから登録解除されたときの処理を記述できる。
	    /// </summary>
	    protected override void OnRemoved()
		{
			base.OnRemoved();
			Scene.RemoveLayer(scrollLayer_);
		}

	    /// <summary>
	    /// オーバーライドして、このレイヤーが破棄されるときの処理を記述できる。
	    /// </summary>
	    protected override void OnDispose()
		{
			base.OnDispose();
			scrollLayer_.Dispose();
		}


        private void ResetOuterBound()
        {
	        var offset = (Selector.ChoiceItems.FirstOrDefault()?.Item as TextureObject2D)?.CenterPosition ??
	                     new Vector2DF(0, 0);
			scrollLayer_.BoundaryStartingPosition = LayoutStarting - offset;
            scrollLayer_.BoundaryEndingPosition = GetSize(layout.Items.Count()) + LayoutStarting - offset;
	        if (ScrollIntoVoid)
	        {
		        scrollLayer_.BoundaryStartingPosition -= GetSize(ExtraLinesOnStarting);
		        scrollLayer_.BoundaryEndingPosition += GetSize(ExtraLinesOnEnding);
	        }
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
            scrollLayer_.BindingAreaRange = new RectF(bindStarting, bindSize);
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
