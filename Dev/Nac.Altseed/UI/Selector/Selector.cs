using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	public interface IActivatableSelectionItem
	{
		void Activate();
		void Disactivate();
	}

	/// <summary>
	/// 選択UIの表示と操作を扱うクラス。
	/// </summary>
	/// <typeparam name="TChoice">選択の結果として返す値の型。</typeparam>
	/// <typeparam name="TAbstractKey">操作に用いる抽象キーを表す値の型。</typeparam>
	public class Selector<TChoice, TAbstractKey> : ReactiveTextureObject2D, ISelector<TChoice, TAbstractKey>
	{
		/// <summary>
		/// 選択肢と選択肢オブジェクトの組。
		/// </summary>
		public class ChoiceItem
		{
			/// <summary>
			/// 選択肢。
			/// </summary>
			public TChoice Choice { get; }
			/// <summary>
			/// 選択肢を表示するオブジェクト。
			/// </summary>
			public Object2D Item { get; }

			/// <summary>
			/// ChoiceItem クラスを生成します。
			/// </summary>
			/// <param name="choice">選択肢。</param>
			/// <param name="item">選択肢を表示するオブジェクト。</param>
			public ChoiceItem(TChoice choice, Object2D item)
			{
				Choice = choice;
				Item = item;
			}
		}

		private readonly List<ChoiceItem> choiceItems_ = new List<ChoiceItem>();
		private readonly Choice<TAbstractKey> choiceSystem_;
		private readonly Subject<Unit> onLayoutChanged_ = new Subject<Unit>();
		private readonly Subject<bool> onActivationStateChanged_ = new Subject<bool>();
		private BooleanDisposable revisingStatus_;
		private IDisposable cancellationOfCursorMoving_;
		private Vector2DF cursorOffset_;
		private bool doWarningAboutKeyBind_ = true;
		private bool isActive_;
		private bool hideSelection_;

		/// <summary>
		/// この選択UIにおける選択肢オブジェクトの配置を決定するレイアウトを取得します。
		/// </summary>
		public Layouter Layout { get; }

		/// <summary>
		/// この選択UIにおいて選択中の選択肢を指し示すカーソルの2Dオブジェクトを取得します。
		/// </summary>
		public ReactiveTextureObject2D Cursor { get; }

		/// <summary>
		/// 選択中の選択肢オブジェクトから見たカーソルの相対位置を取得または設定します。
		/// </summary>
		public Vector2DF CursorOffset
		{
			get { return cursorOffset_; }
			set
			{
				cursorOffset_ = value;
				if (choiceSystem_.SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
				{
					SuddenlyMoveCursor(choiceItems_[choiceSystem_.SelectedIndex].Item);
				}
			}
		}

		/// <summary>
		/// カーソルを移動した時のアニメーションを IObservable&lt;Vector2DF&gt; として取得または設定します。
		/// </summary>
		public Func<Object2D, Vector2DF, IObservable<Vector2DF>> SetCursorPosition { get; set; }

		/// <summary>
		/// この選択UIで用いる Controller を自動的に Update するかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool IsControllerUpdated
		{
			get { return choiceSystem_.IsControllerUpdated; }
			set { choiceSystem_.IsControllerUpdated = value; }
		}

		/// <summary>
		/// この選択UIの始端と終端をループするかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool Loop
		{
			get { return choiceSystem_.Loop; }
			set { choiceSystem_.Loop = value; }
		}

		/// <summary>
		/// この選択UIが操作中(アクティブ)であるかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool IsActive
		{
			get { return isActive_; }
			set
			{
				if (isActive_ != value)
				{
					isActive_ = value;
					onActivationStateChanged_.OnNext(value);
				}
			}
		}

		/// <summary>
		/// 選択状態を隠すかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool HideSelection
		{
			get { return hideSelection_; }
			set
			{
				hideSelection_ = value;
				if (!value)
				{
					Cursor.IsDrawn = GetChoiceItemAt(SelectedIndex) != null;
					(SelectedItem as IActivatableSelectionItem)?.Activate();
				}
				else
				{
					Cursor.IsDrawn = false;
					foreach (var choiceItem in ChoiceItems)
					{
						(choiceItem.Item as IActivatableSelectionItem)?.Disactivate();
					}
				}
			}
		}

		/// <summary>
		/// この選択UIに登録されている、選択肢と選択肢オブジェクトの組のコレクションを取得します。
		/// </summary>
		public IReadOnlyList<ChoiceItem> ChoiceItems => choiceItems_;

		public IEnumerable<TChoice> AvailableChoices => ChoiceItems.Select(x => x.Choice);

		/// <summary>
		/// 選択中の選択肢のインデックスを取得します。
		/// </summary>
		public int SelectedIndex { get; private set; }

		/// <summary>
		/// 選択中の選択肢を取得します。
		/// </summary>
		public TChoice SelectedChoice
		{
			get
			{
				var choiceItem = GetChoiceItemAt(SelectedIndex);
				return choiceItem != null ? choiceItem.Choice : default(TChoice);
			}
		}

		/// <summary>
		/// 選択中の選択肢オブジェクトを取得します。
		/// </summary>
		public Object2D SelectedItem => GetChoiceItemAt(SelectedIndex)?.Item;

		/// <summary>
		/// 選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnSelectionChanged { get; }
		/// <summary>
		/// ユーザーの入力によって選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnMove { get; }
		/// <summary>
		/// 選択が確定されたことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnDecide { get; }
		/// <summary>
		/// 選択がキャンセルされたことを通知するイベントを取得または設定します。
		/// </summary>
		public IObservable<TChoice> OnCancel { get; }
		/// <summary>
		/// 選択肢のレイアウトに変化があったときに通知するイベントを取得します。
		/// </summary>
		public IObservable<Unit> OnLayoutChanged => onLayoutChanged_;
		/// <summary>
		/// この選択UIのアクティブ状態が変化した時に通知するイベントを取得します。
		/// </summary>
		public IObservable<bool> OnActivationStateChanged => onActivationStateChanged_;


		/// <summary>
		/// 選択UIを初期化します。
		/// </summary>
		/// <param name="controller">この選択UIの操作に用いるコントローラー。</param>
		/// <param name="layout">この選択UIにおける選択肢オブジェクトの配置に用いるレイアウト。</param>
		public Selector(Controller<TAbstractKey> controller, Layouter layout)
		{
			IsActive = false;
			IsDrawn = false;
			revisingStatus_ = new BooleanDisposable();
			revisingStatus_.Dispose();
			Layout = layout;

			choiceSystem_ = new Choice<TAbstractKey>(0, controller);
			OnSelectionChanged = choiceSystem_.OnSelectionChanged
				.TakeWhile(x => IsAlive)
				.Where(x => revisingStatus_.IsDisposed)
				.Where(x => x >= 0)
				.Select(e => choiceItems_[e].Choice);
			OnMove = choiceSystem_.OnMove
				.TakeWhile(x => IsAlive)
				.Where(x => x >= 0)
				.Select(e => choiceItems_[e].Choice);
			OnDecide = choiceSystem_.OnDecide
				.TakeWhile(x => IsAlive)
				.Where(x => x >= 0)
				.Select(e => choiceItems_[e].Choice);
			OnCancel = choiceSystem_.OnCancel
				.TakeWhile(x => IsAlive)
				.Where(x => x >= 0)
				.Select(e => choiceItems_[e].Choice);

			choiceSystem_.OnSelectionChanged.Subscribe(OnSelectionChangedHandler);
			SelectedIndex = choiceSystem_.SelectedIndex;

			Cursor = new ReactiveTextureObject2D();
			Cursor.IsDrawn = false;

			AddDrawnChild(Layout,
				ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal,
				ChildTransformingMode.All,
				ChildDrawingMode.DrawingPriority);
		}

		/// <summary>
		/// UIに選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="item">追加する選択肢を表示する2Dオブジェクト。</param>
		public virtual void AddChoice(TChoice choice, Object2D item)
		{
			ThrowIfDisposed();
			Layout.AddItem(item);
			choiceItems_.Add(new ChoiceItem(choice, item));
			choiceSystem_.Size++;
			onLayoutChanged_.OnNext(Unit.Default);
		}
		
		/// <summary>
		/// UIに選択肢を挿入します。
		/// </summary>
		/// <param name="index">挿入する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="item">挿入する選択肢を表示する2Dオブジェクト。</param>
		public virtual void InsertChoice(int index, TChoice choice, Object2D item)
		{
			ThrowIfDisposed();
			Layout.InsertItem(index, item);
			choiceItems_.Insert(index, new ChoiceItem(choice, item));
			choiceSystem_.Size++;
			if (index <= SelectedIndex)
			{
				using (revisingStatus_ = new BooleanDisposable())
				{
					choiceSystem_.SelectedIndex++;
				}
			}
			onLayoutChanged_.OnNext(Unit.Default);
		}

		/// <summary>
		/// UIから選択肢を削除します。
		/// </summary>
		/// <param name="choice">削除する選択肢。</param>
		public virtual Object2D RemoveChoice(TChoice choice)
		{
			ThrowIfDisposed();
			var index = choiceItems_.IndexOf(c => c.Choice.Equals(choice));
			if (index != -1)
			{
				var item = choiceItems_[index].Item;
				choiceItems_.RemoveAt(index);
				if (index == 0 && SelectedIndex == 0)
				{
					OnSelectionChangedHandler(0);
				}
				else if (index <= SelectedIndex)
				{
					using (revisingStatus_ = new BooleanDisposable())
					{
						choiceSystem_.SelectedIndex--;
					}
				}
				choiceSystem_.Size--;
				Layout.RemoveItem(item);
				onLayoutChanged_.OnNext(Unit.Default);
				return item;
			}
			return null;
		}

		/// <summary>
		/// UIからすべての選択肢を削除します。
		/// </summary>
		public virtual void ClearChoice()
		{
			ThrowIfDisposed();
			choiceItems_.Clear();
			choiceSystem_.Size = 0;
			Layout.ClearItem();
			onLayoutChanged_.OnNext(Unit.Default);
		}

		/// <summary>
		/// 選択操作と抽象キーとを対応付けます。
		/// </summary>
		/// <param name="next">次の選択肢に移動する操作に対応付ける抽象キー。</param>
		/// <param name="prev">前の選択肢に移動する操作に対応付ける抽象キー。</param>
		/// <param name="decide">決定操作に対応付ける抽象キー。</param>
		/// <param name="cancel">キャンセル操作に対応付ける抽象キー。</param>
		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
		{
			choiceSystem_.BindKey(next, ChoiceControl.Next);
			choiceSystem_.BindKey(prev, ChoiceControl.Previous);
			choiceSystem_.BindKey(decide, ChoiceControl.Decide);
			choiceSystem_.BindKey(cancel, ChoiceControl.Cancel);
			doWarningAboutKeyBind_ = false;
		}

		/// <summary>
		/// カーソルや選択肢の追加・削除が起きたときに滑らかにアニメーションするよう準備します。
		/// </summary>
		/// <param name="start">アニメーション開始速度。</param>
		/// <param name="end">アニメーション終了速度。</param>
		/// <param name="time">アニメーションにかける時間。</param>
		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			SetCursorPosition = (o, target) =>
			{
				var f = Easing.GetEasingFunction(start, end);
				return OnUpdateEvent.Select(u => o.Position)
					.Select((v, i) => Easing.GetNextValue(v, target, i, time, f))
					.Take(time + 1);
			};
		}

		/// <summary>
		/// 選択の際にスキップする選択肢を指定します。
		/// </summary>
		/// <param name="choice">スキップする選択肢。</param>
		public void AddSkippedChoice(TChoice choice)
		{
			var index = choiceItems_.IndexOf(x => x.Choice.Equals(choice));
			choiceSystem_.AddSkippedIndex(index);
		}

		/// <summary>
		/// 指定した選択肢を表示している2Dオブジェクトを取得します。
		/// </summary>
		/// <param name="choice">調べる選択肢。</param>
		/// <returns>選択肢を表示している2Dオブジェクト。</returns>
		public Object2D GetItemForChocie(TChoice choice)
		{
			return choiceItems_.Find(x => x.Choice.Equals(choice))?.Item;
		}

		/// <summary>
		/// 選択中の選択肢を変更します。
		/// </summary>
		/// <param name="index">変更先のインデックス。</param>
		public void ChangeSelection(int index)
		{
			choiceSystem_.SelectedIndex = index;
		}

		/// <summary>
		/// オーバーライドして、この2Dオブジェクトがレイヤーに登録されたときの処理を記述できる。
		/// </summary>
		protected override void OnAdded()
		{
			base.OnAdded();
			Layer.AddObject(Cursor);
		}

		/// <summary>
		/// オーバーライドして、この2Dオブジェクトがレイヤーから登録解除されたときの処理を記述できる。
		/// </summary>
		protected override void OnRemoved()
		{
			base.OnRemoved();
			Cursor.Layer.RemoveObject(Cursor);
		}

		/// <summary>
		/// オーバーライドして、この2Dオブジェクトが破棄される際の処理を記述できる。
		/// </summary>
		protected override void OnDispose()
		{
			IsActive = false;
			base.OnDispose();
			Cursor.Dispose();
			onLayoutChanged_.OnCompleted();
		}

		/// <summary>
		/// オーバーライドして、この2Dオブジェクトの更新処理を記述できる。
		/// </summary>
		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (!IsAlive)
			{
				return;
			}

			var beVanished = new List<TChoice>();
			foreach (var item in choiceItems_)
			{
				if (!item.Item.IsAlive)
				{
					beVanished.Add(item.Choice);
				}
			}
			beVanished.ForEach(x => RemoveChoice(x));

			if (IsActive)
			{
				choiceSystem_.Update();
				if (doWarningAboutKeyBind_)
				{
					Debug.WriteWarning(this, $"キーと操作の対応付けが設定されていません。このセレクターは操作できません。{nameof(BindKey)}メソッドを呼び出してキーと操作を対応付けてください。");
					doWarningAboutKeyBind_ = false;
				}
			}
		}


		private void OnSelectionChangedHandler(int index)
		{
			var prev = GetChoiceItemAt(SelectedIndex);
			var next = GetChoiceItemAt(index);

			if (!HideSelection)
			{
				(prev?.Item as IActivatableSelectionItem)?.Disactivate();
				(next?.Item as IActivatableSelectionItem)?.Activate();
			}

			if (next != null)
			{
				if (!HideSelection)
				{
					Cursor.IsDrawn = true;
				}
				if (prev != null)
				{
					MoveCursor(next.Item);
				}
				else
				{
					SuddenlyMoveCursor(next.Item);
				}
			}
			else
			{
				Cursor.IsDrawn = false;
			}

			SelectedIndex = choiceSystem_.SelectedIndex;
		}

		private ChoiceItem GetChoiceItemAt(int index)
		{
			if (index == Choice<TAbstractKey>.DisabledIndex || index >= choiceItems_.Count)
			{
				return null;
			}
			return choiceItems_[index];
		}

		private void SuddenlyMoveCursor(Object2D obj)
		{
			cancellationOfCursorMoving_?.Dispose();
			cancellationOfCursorMoving_ = null;

			Cursor.Parent?.RemoveChild(Cursor);
			obj.AddChild(Cursor, ChildManagementMode.Nothing, ChildTransformingMode.All);
			Cursor.Position = CursorOffset;
		}

		private void MoveCursor(Object2D obj)
		{
			if (SetCursorPosition == null || Cursor.Parent == null)
			{
				SuddenlyMoveCursor(obj);
			}
			else
			{
				cancellationOfCursorMoving_?.Dispose();

				Cursor.Position = Cursor.GetGlobalPosition() - obj.GetGlobalPosition();
				Cursor.Parent?.RemoveChild(Cursor);
				obj.AddChild(Cursor, ChildManagementMode.Nothing, ChildTransformingMode.All);
				cancellationOfCursorMoving_ = SetCursorPosition(Cursor, CursorOffset)
					.Subscribe(p => Cursor.Position = p, () => { cancellationOfCursorMoving_ = null; });
			}
		}

		private void ThrowIfDisposed()
		{
			if (!IsAlive)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
	}
}