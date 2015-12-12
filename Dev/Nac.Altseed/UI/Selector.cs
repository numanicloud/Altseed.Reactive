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

namespace Nac.Altseed.UI
{
	public interface IActivatableSelectionItem
	{
		void Activate();
		void Disactivate();
	}

	public class Selector<TChoice, TAbstractKey> : TextureObject2D, ISelector<TChoice, TAbstractKey>
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
		private IDisposable cancellationOfCursorMoving = null;
		private Vector2DF cursorOffset_ = new Vector2DF();
        private BooleanDisposable revisingStatus;
        private Layouter layout;
		private Subject<Unit> onLayoutChanged_ = new Subject<Unit>();

		public bool IsActive { get; set; }
		public int SelectedIndex { get; private set; }
		public IObservable<Unit> OnLayoutChanged => onLayoutChanged_;
		public IObservable<TChoice> OnSelectionChanged { get; private set; }
		public IObservable<TChoice> OnMove { get; private set; }
		public IObservable<TChoice> OnDecide { get; private set; }
		public IObservable<TChoice> OnCancel { get; private set; }
		public IReadOnlyList<ChoiceItem> ChoiceItems => choiceItems_;
        
		public Vector2DF CursorOffset
		{
			get { return cursorOffset_; }
			set
			{
				cursorOffset_ = value;
				if(choiceSystem.SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
				{
					cancellationOfCursorMoving?.Dispose();
					Position = choiceItems_[choiceSystem.SelectedIndex].Item.Position + cursorOffset_;
				}
			}
		}
		public Func<Object2D, Object2D, IObservable<Vector2DF>> SetCursorPosition { get; set; }
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
            IsDrawn = false;
            revisingStatus = new BooleanDisposable();
            revisingStatus.Dispose();
            this.layout = layout;

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
            
            SetCursorPosition = (o, target) => Observable.Return(target.Position);
        }


		public void SetEasingBehaviorUp(EasingStart start, EasingEnd end, int time)
		{
			SetCursorPosition = (o, target) =>
			{
				var f = Easing.GetEasingFunction(start, end);
				return UpdateManager.Instance.FrameUpdate
					.Select(u => o.Position)
					.Select((v, i) => Easing.GetNextValue(v, target.Position, i, time, f))
					.Take(time + 1);
			};
		}

		public void AddChoice(TChoice choice, Object2D item)
		{
            layout.AddItem(item);
			choiceItems_.Add(new ChoiceItem(choice, item));
			choiceSystem.Size++;
			onLayoutChanged_.OnNext(Unit.Default);
		}

        public void InsertChoice(int index, TChoice choice, Object2D item)
        {
            layout.InsertItem(index, item);
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
				layout.RemoveItem(item);
				choiceItems_.RemoveAt(index);
                if(index <= SelectedIndex)
                {
                    using(revisingStatus = new BooleanDisposable())
                    {
                        choiceSystem.SelectedIndex--;
                    }
                }
                choiceSystem.Size--;
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
            layout.ClearItem();
            choiceItems_.Clear();
            choiceSystem.Size = 0;
			onLayoutChanged_.OnNext(Unit.Default);
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

        
        protected override void OnUpdate()
        {
            if(IsActive)
            {
                choiceSystem.Update();
            }

            if(cancellationOfCursorMoving == null && SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
            {
                Position = choiceItems_[SelectedIndex].Item.Position;
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
			if(SelectedIndex != Choice<TAbstractKey>.DisabledIndex && SelectedIndex < choiceItems_.Count)
			{
				(choiceItems_[SelectedIndex].Item as IActivatableSelectionItem)?.Disactivate();
			}

			if(index != Choice<TAbstractKey>.DisabledIndex)
			{
				IsDrawn = true;
				(choiceItems_[index].Item as IActivatableSelectionItem)?.Activate();
                if(SelectedIndex == Choice<TAbstractKey>.DisabledIndex)
                {
                    Position = choiceItems_[index].Item.Position;
                }
                else
                {
                    MoveCursor(choiceItems_[index].Item);
                }
			}
			else
			{
				IsDrawn = false;
			}

			SelectedIndex = choiceSystem.SelectedIndex;
		}

		private void MoveCursor(Object2D obj)
		{
			cancellationOfCursorMoving?.Dispose();
			cancellationOfCursorMoving = SetCursorPosition?.Invoke(this, obj)
                .Subscribe(p => Position = p, () => cancellationOfCursorMoving = null);
		}
	}
}
