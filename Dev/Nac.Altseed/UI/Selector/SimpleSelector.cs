using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI.Selector
{
	public class SimpleSelector<TChoice> : ReactiveTextureObject2D, ISelector<TChoice>
	{
		private enum SelectorControl
		{
			Next, Previous, Decide, Cancel
		}

		private readonly LinearPanel layout_;
		private readonly KeyboardController<SelectorControl> controller_;
		private readonly Selector<TChoice, SelectorControl> selector_;

		public SimpleSelector()
		{
			IsDrawn = false;
			layout_ = new LinearPanel();
			controller_ = new KeyboardController<SelectorControl>();

			selector_ = new Selector<TChoice, SelectorControl>(controller_, layout_);
			selector_.IsActive = true;
			selector_.BindKey(SelectorControl.Next, SelectorControl.Previous, SelectorControl.Decide, SelectorControl.Cancel);

			AddDrawnChild(selector_,
				ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal | ChildManagementMode.IsUpdated,
				ChildTransformingMode.All,
				ChildDrawingMode.Color | ChildDrawingMode.DrawingPriority);
		}

		/// <summary>
		/// キーボードのキーにこの選択UIの操作を割り当てます。
		/// </summary>
		/// <param name="keyToNext">次の項目へ移動するためのキー。</param>
		/// <param name="keyToPrevious">前の項目に移動するためのキー。</param>
		/// <param name="keyToDecide">項目を決定するためのキー。</param>
		/// <param name="keyToCancel">選択をキャンセルするためのキー。</param>
		public void BindKey(Keys keyToNext, Keys keyToPrevious, Keys keyToDecide, Keys keyToCancel)
		{
			controller_.BindKey(keyToNext, SelectorControl.Next);
			controller_.BindKey(keyToPrevious, SelectorControl.Previous);
			controller_.BindKey(keyToDecide, SelectorControl.Decide);
			controller_.BindKey(keyToCancel, SelectorControl.Cancel);
		}

		public Vector2DF ItemSpan
		{
			get { return layout_.ItemSpan; }
			set { layout_.ItemSpan = value; }
		}

		public ReactiveTextureObject2D Cursor => selector_.Cursor;

		#region Selectorへの委譲
		/// <summary>
		/// UIに選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="item">追加する選択肢を表示する2Dオブジェクト。</param>
		public void AddChoice(TChoice choice, Object2D item)
		{
			selector_.AddChoice(choice, item);
		}

		/// <summary>
		/// UIから選択肢を削除します。
		/// </summary>
		/// <param name="choice">削除する選択肢。</param>
		public Object2D RemoveChoice(TChoice choice)
		{
			return selector_.RemoveChoice(choice);
		}

		/// <summary>
		/// UIに選択肢を挿入します。
		/// </summary>
		/// <param name="index">挿入する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="item">挿入する選択肢を表示する2Dオブジェクト。</param>
		public void InsertChoice(int index, TChoice choice, Object2D item)
		{
			selector_.InsertChoice(index, choice, item);
		}

		/// <summary>
		/// UIからすべての選択肢を削除します。
		/// </summary>
		public void ClearChoice()
		{
			selector_.ClearChoice();
		}

		/// <summary>
		/// この選択UIが操作中(アクティブ)であるかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool IsActive
		{
			get { return selector_.IsActive; }
			set { selector_.IsActive = value; }
		}

		/// <summary>
		/// この選択UIに登録されている選択肢のコレクションを取得します。
		/// </summary>
		public IEnumerable<TChoice> AvailableChoices
		{
			get { return selector_.AvailableChoices; }
		}

		/// <summary>
		/// 選択中の選択肢のインデックスを取得または設定します。
		/// </summary>
		public int SelectedIndex
		{
			get { return selector_.SelectedIndex; }
		}

		/// <summary>
		/// 選択中の選択肢を取得します。
		/// </summary>
		public TChoice SelectedChoice
		{
			get { return selector_.SelectedChoice; }
		}

		/// <summary>
		/// 選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnSelectionChanged
		{
			get { return selector_.OnSelectionChanged; }
		}

		/// <summary>
		/// ユーザーの入力によって選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnMove
		{
			get { return selector_.OnMove; }
		}

		/// <summary>
		/// 選択が確定されたことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnDecide
		{
			get { return selector_.OnDecide; }
		}

		/// <summary>
		/// 選択がキャンセルされたことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnCancel
		{
			get { return selector_.OnCancel; }
		}

		/// <summary>
		/// この選択UIのアクティブ状態が変化した時に通知するイベントを取得します。
		/// </summary>
		public IObservable<bool> OnActivationStateChanged
		{
			get { return selector_.OnActivationStateChanged; }
		}
		#endregion
	}
}
