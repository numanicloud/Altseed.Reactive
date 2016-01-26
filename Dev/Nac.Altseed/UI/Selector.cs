﻿using System;
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

	public class Selector<TChoice, TAbstractKey> : ISelector<TChoice, TAbstractKey>
	{
		public class ChoiceItem
		{
			public TChoice Choice { get; private set; }
			public Object2D Item { get; private set; }

			public ChoiceItem(TChoice choice, Object2D item)
			{
				Choice = choice;
				Item = item;
			}
		}

		private List<ChoiceItem> choiceItems_ = new List<ChoiceItem>();
		private Choice<TAbstractKey> choiceSystem;
		private IDisposable cancellationOfCursorMoving = null, update = null;
		private Vector2DF cursorOffset_ = new Vector2DF();
		private Subject<Unit> onLayoutChanged_ = new Subject<Unit>();
        private BooleanDisposable revisingStatus;

		public bool IsActive { get; set; }
		public int SelectedIndex { get; private set; }
		public IObservable<Unit> OnLayoutChanged => onLayoutChanged_;
		public IObservable<TChoice> OnSelectionChanged { get; private set; }
		public IObservable<TChoice> OnMove { get; private set; }
		public IObservable<TChoice> OnDecide { get; private set; }
		public IObservable<TChoice> OnCancel { get; private set; }
		public IReadOnlyList<ChoiceItem> ChoiceItems => choiceItems_;
        
        public Layouter Layout { get; private set; }
		public ReactiveTextureObject2D Cursor { get; private set; }
		public Vector2DF CursorOffset
		{
			get { return cursorOffset_; }
			set
			{
				cursorOffset_ = value;
				if(choiceSystem.SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
				{
                    SuddenlyMoveCursor(choiceItems_[choiceSystem.SelectedIndex].Item);
				}
			}
		}
        public Func<Object2D, Vector2DF, IObservable<Vector2DF>> SetCursorPosition { get; set; }
        public bool IsControllerUpdated
        {
            get { return choiceSystem.IsControllerUpdated; }
            set { choiceSystem.IsControllerUpdated = value; }
        }
        public bool Loop
        {
            get { return choiceSystem.Loop; }
            set { choiceSystem.Loop = value; }
        }

		public TChoice SelectedChoice
		{
			get
			{
				if(SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
				{
					return ChoiceItems[SelectedIndex].Choice;
				}
				else
				{
					return default(TChoice);
				}
			}
		}

		public Selector(Controller<TAbstractKey> controller, Layouter layout)
		{
			IsActive = true;
            revisingStatus = new BooleanDisposable();
            revisingStatus.Dispose();
            this.Layout = layout;

			choiceSystem = new Choice<TAbstractKey>(0, controller);
			OnSelectionChanged = choiceSystem.OnSelectionChanged
                .Where(x => revisingStatus.IsDisposed)
                .Where(x => x >= 0)
                .Select(e => choiceItems_[e].Choice);
			OnMove = choiceSystem.OnMove
                .Where(x => x >= 0)
                .Select(e => choiceItems_[e].Choice);
			OnDecide = choiceSystem.OnDecide
                .Where(x => x >= 0)
                .Select(e => choiceItems_[e].Choice);
			OnCancel = choiceSystem.OnCancel
                .Where(x => x >= 0)
                .Select(e => choiceItems_[e].Choice);

			choiceSystem.OnSelectionChanged.Subscribe(OnSelectionChangedHandler);
			SelectedIndex = choiceSystem.SelectedIndex;

			Cursor = new ReactiveTextureObject2D();
			Cursor.IsDrawn = false;
		}


		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			SetCursorPosition = (o, target) =>
			{
				var f = Easing.GetEasingFunction(start, end);
				return UpdateManager.Instance.FrameUpdate
					.Select(u => o.Position)
					.Select((v, i) => Easing.GetNextValue(v, target, i, time, f))
					.Take(time + 1);
			};
		}

		public void AddChoice(TChoice choice, Object2D item)
		{
            Layout.AddItem(item);
			choiceItems_.Add(new ChoiceItem(choice, item));
			choiceSystem.Size++;
			onLayoutChanged_.OnNext(Unit.Default);
		}

        public void InsertChoice(int index, TChoice choice, Object2D item)
        {
            Layout.InsertItem(index, item);
            choiceItems_.Insert(index, new ChoiceItem(choice, item));
            if(index <= SelectedIndex)
            {
                using(revisingStatus = new BooleanDisposable())
                {
                    choiceSystem.SelectedIndex++;
                }
            }
            choiceSystem.Size++;
			onLayoutChanged_.OnNext(Unit.Default);
		}

		public Object2D RemoveChoice(TChoice choice)
		{
			var index = choiceItems_.IndexOf(c => c.Choice.Equals(choice));
            if(index != -1)
			{
				var item = choiceItems_[index].Item;
				choiceItems_.RemoveAt(index);
                if(index <= SelectedIndex)
                {
                    using(revisingStatus = new BooleanDisposable())
                    {
                        choiceSystem.SelectedIndex--;
                    }
                }
                choiceSystem.Size--;
				Layout.RemoveItem(item);
				onLayoutChanged_.OnNext(Unit.Default);
				return item;
			}
			else
			{
				return null;
			}
		}

        public void ClearChoice()
        {
            choiceItems_.Clear();
            choiceSystem.Size = 0;
            Layout.ClearItem();
			onLayoutChanged_.OnNext(Unit.Default);
		}

		public void AddSkippedChoice(TChoice choice)
		{
			var index = choiceItems_.IndexOf(x => x.Choice.Equals(choice));
			choiceSystem.AddSkippedIndex(index);
		}

		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
		{
			choiceSystem.BindKey(next, ChoiceControl.Next);
			choiceSystem.BindKey(prev, ChoiceControl.Previous);
			choiceSystem.BindKey(decide, ChoiceControl.Decide);
			choiceSystem.BindKey(cancel, ChoiceControl.Cancel);
		}

		public Object2D GetItemForChocie(TChoice choice)
		{
			return choiceItems_.Find(x => x.Choice.Equals(choice))?.Item;
		}


		/// <summary>
		/// 指定したレイヤーにこのマネージャーを登録します。
		/// </summary>
		/// <param name="layer">このマネージャーを登録するレイヤー。</param>
		public void RegisterLayer(ReactiveLayer2D layer)
		{
			layer.AddObject(Layout);
			layer.AddObject(Cursor);
			update = layer.OnUpdateEvent.Subscribe(f => Update());
		}

		/// <summary>
		/// 指定したレイヤーからこのマネージャーを登録解除します。
		/// </summary>
		/// <param name="layer">このマネージャーを登録解除するレイヤー。</param>
		public void UnregisterLayer(ReactiveLayer2D layer)
		{
			layer.RemoveObject(Layout);
			layer.RemoveObject(Cursor);
			update?.Dispose();
		}

		public void Dispose()
		{
			Layout.Dispose();
			Cursor.Dispose();
			update?.Dispose();
		}
        
        private void Update()
        {
            if(IsActive)
            {
                choiceSystem.Update();
            }

            var beVanished = new List<TChoice>();
            foreach(var item in choiceItems_)
            {
                if(!item.Item.IsAlive)
                {
                    beVanished.Add(item.Choice);
                }
            }
            beVanished.ForEach(x => RemoveChoice(x));
        }


        private void OnSelectionChangedHandler(int index)
		{
			if(SelectedIndex != Choice<TAbstractKey>.DisabledIndex
				&& SelectedIndex < choiceItems_.Count
				&& choiceItems_[SelectedIndex].Item.IsAlive)
			{
				(choiceItems_[SelectedIndex].Item as IActivatableSelectionItem)?.Disactivate();
			}

			if(index != Choice<TAbstractKey>.DisabledIndex && choiceItems_[index].Item.IsAlive)
			{
				Cursor.IsDrawn = true;
				(choiceItems_[index].Item as IActivatableSelectionItem)?.Activate();
                if(SelectedIndex == Choice<TAbstractKey>.DisabledIndex)
                {
                    SuddenlyMoveCursor(choiceItems_[index].Item);
                }
                else
                {
                    MoveCursor(choiceItems_[index].Item);
                }
			}
			else
			{
				Cursor.IsDrawn = false;
			}

			SelectedIndex = choiceSystem.SelectedIndex;
		}

        private void SuddenlyMoveCursor(Object2D obj)
        {
            cancellationOfCursorMoving?.Dispose();
            cancellationOfCursorMoving = null;

			Cursor.Parent?.RemoveChild(Cursor);
            obj.AddChild(Cursor, ChildManagementMode.Nothing, ChildTransformingMode.All);
            Cursor.Position = CursorOffset;
        }

		private void MoveCursor(Object2D obj)
		{
            if(SetCursorPosition == null)
            {
                SuddenlyMoveCursor(obj);
            }
            else
            {
                cancellationOfCursorMoving?.Dispose();
                
                Cursor.Position = Cursor.GetGlobalPosition() - obj.GetGlobalPosition();
				Cursor.Parent?.RemoveChild(Cursor);
				obj.AddChild(Cursor, ChildManagementMode.Nothing, ChildTransformingMode.All);
                cancellationOfCursorMoving = SetCursorPosition(Cursor, CursorOffset)
                    .Subscribe(p => Cursor.Position = p, () => cancellationOfCursorMoving = null);
            }
		}
	}
}
