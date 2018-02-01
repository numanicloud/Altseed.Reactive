using System;
using System.Linq;
using System.Reactive.Subjects;
using asd;
using Altseed.Reactive.Ui.ChoiceList;
using Altseed.Reactive.Ui.Cursor;
using System.Reactive.Linq;
using System.Reactive;
using Altseed.Reactive.Object;
using Altseed.Reactive.Input;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// 選択肢UIを提供するクラス。
	/// </summary>
	/// <typeparam name="TChoice"></typeparam>
	public class SelectorFacade<TChoice> : ReactiveTextureObject2D
	{
		private static readonly string CursorComponentKey = "SelectorFacade_Cursor";

		private LinearPanel Layout { get; set; }
		private LinearChoiceList<TChoice> ChoiceList { get; set; }
		private Object2D cursor_;
		private CursorComponent cursorComponent_;

		/// <summary>
		/// この選択肢UIにカーソルを表示するオブジェクトを取得または設定します。
		/// </summary>
		public Object2D Cursor
		{
			get => cursor_;
			set
			{
				cursor_.RemoveComponent(CursorComponentKey);
				cursor_ = value;
				cursor_.AddComponent(CursorComponent, CursorComponentKey);
			}
		}
		/// <summary>
		/// この選択肢UIのカーソルを制御するコンポーネントを取得または設定します。
		/// </summary>
		public CursorComponent CursorComponent
		{
			get => cursorComponent_;
			set
			{
				Cursor.RemoveComponent(CursorComponentKey);
				cursorComponent_ = value;
				Cursor.AddComponent(value, CursorComponentKey);
			}
		}
		/// <summary>
		/// この選択肢UIで現在選択されているインデックスを取得します。
		/// </summary>
		public int SelectedIndex => ChoiceList.SelectedIndex;
		/// <summary>
		/// この選択肢UIで現在選択されているオブジェクトを取得します。
		/// </summary>
		public Object2D SelectedItem => Layout.Items.ElementAt(ChoiceList.SelectedIndex).Object;
		/// <summary>
		/// この選択肢UIで現在選択されている選択肢を取得します。
		/// </summary>
		public TChoice SelectedChoice => ChoiceList.SelectedChoice;
		/// <summary>
		/// この選択肢UIの要素同士の間隔を取得又は設定します。
		/// </summary>
		public Vector2DF ItemSpan
		{
			get => Layout.ItemSpan;
			set => Layout.ItemSpan = value;
		}

		/// <summary>
		/// 選択が変更されたことを通知するストリーム。
		/// </summary>
		public IObservable<int> OnSelectionChanged { get; }
		private Subject<Unit> ReviseTester { get; set; }

		private SelectorFacade()
		{
			Layout = new LinearPanel();
			ChoiceList = new LinearChoiceList<TChoice>();
			Cursor = new TextureObject2D()
			{
				Scale = new Vector2DF(25, 25)
			};
			CursorComponent = new EasingCursorComponent();
			Cursor.AddComponent(CursorComponent, CursorComponentKey);
			
			ReviseTester = new Subject<Unit>();
			IsDrawn = false;

			OnSelectionChanged = ReviseTester.Select(x => ChoiceList.SelectedIndex)
				.DistinctUntilChanged();

			OnSelectionChanged.Subscribe(x =>
				{
					CursorComponent.IsHidden = ChoiceList.SelectedIndex != ChoiceList.DisabledIndex;
					CursorComponent.MoveTo(Layout.Items.ElementAt(ChoiceList.SelectedIndex).Object);
				});

			if (Layout.Parent == null)
			{
				AddDrawnChild(Layout,
					ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal | ChildManagementMode.IsUpdated,
					ChildTransformingMode.All,
					ChildDrawingMode.Color | ChildDrawingMode.DrawingPriority);
			}
		}

		/// <summary>
		/// この選択肢UIに選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="obj">選択肢の内容を表示するオブジェクト。</param>
		/// <param name="skipped">この選択肢がスキップされるかどうかを表す真偽値。</param>
		public void AddChoice(TChoice choice, Object2D obj, bool skipped = false)
		{
			Layout.AddItem(obj);
			ChoiceList.AddChoice(choice, skipped);
			ReviseTester.OnNext(Unit.Default);
		}

		/// <summary>
		/// この選択肢UIに選択肢を挿入します。
		/// </summary>
		/// <param name="index">選択する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="obj">選択肢の内容を表示するオブジェクト。</param>
		/// <param name="skipped">この選択肢がスキップされるかどうかを表す真偽値。</param>
		public void InsertChoice(int index, TChoice choice, Object2D obj, bool skipped = false)
		{
			Layout.InsertItem(index, obj);
			ChoiceList.AddChoice(choice, skipped);
			ReviseTester.OnNext(Unit.Default);
		}

		/// <summary>
		/// この選択肢UIから選択肢を削除します。
		/// </summary>
		/// <param name="choice">削除する選択肢。</param>
		/// <returns>選択肢が削除されたかどうか。追加されていないインスタンスを指定したときに<c>false</c>となります。</returns>
		public bool RemoveChoice(TChoice choice)
		{
			Layout.RemoveItem(SelectedItem);
			var result = ChoiceList.RemoveChoice(choice);

			ReviseTester.OnNext(Unit.Default);

			return result;
		}

		/// <summary>
		/// この選択肢UIから選択肢をクリアします。
		/// </summary>
		public void ClearChoice()
		{
			Layout.ClearItem();
			ChoiceList.ClearChoice();
			ReviseTester.OnNext(Unit.Default);
		}

		/// <summary>
		/// 前の選択肢に移動します。
		/// </summary>
		/// <returns></returns>
		public bool SelectPreviousIndex()
		{
			var result = ChoiceList.SelectPreviousIndex();
			ReviseTester.OnNext(Unit.Default);
			return result;
		}

		/// <summary>
		/// 次の選択肢に移動します。
		/// </summary>
		/// <returns></returns>
		public bool SelectNextIndex()
		{
			var result = ChoiceList.SelectNextIndex();
			ReviseTester.OnNext(Unit.Default);
			return result;
		}
		

		/// <summary>
		/// この選択肢UIをコントローラーで操作できるようにします。
		/// </summary>
		/// <typeparam name="TControl">コントローラーの操作を表す型。</typeparam>
		/// <param name="controller">この選択肢UIを操作するコントローラー。</param>
		/// <param name="controlPrev">前に移動する操作を表す値。</param>
		/// <param name="controlNext">次に移動する操作を表す値。</param>
		public void BindController<TControl>(Controller<TControl> controller,
			TControl controlPrev,
			TControl controlNext)
		{
			OnUpdatedEvent.Where(x => controller.GetState(controlPrev) == InputState.Push)
				.Subscribe(x => SelectPreviousIndex());
			OnUpdatedEvent.Where(x => controller.GetState(controlNext) == InputState.Push)
				.Subscribe(x => SelectNextIndex());
		}

		/// <summary>
		/// この選択肢UIをキーボードで操作できるようにします。
		/// </summary>
		/// <param name="keyToPrev">前に移動するキー。</param>
		/// <param name="keyToNext">次に移動するキー。</param>
		public void BindKeyboard(Keys keyToPrev, Keys keyToNext)
		{
			var controller = new KeyboardController<int>();
			controller.BindKey(keyToPrev, 0);
			controller.BindKey(keyToNext, 1);
			BindController(controller, 0, 1);
		}
	}
}
