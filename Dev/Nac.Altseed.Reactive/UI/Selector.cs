using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;

namespace Nac.Altseed.Reactive.UI
{
	public interface IActivatableSelectionItem
	{
		void Activate();
		void Disactivate();
	}

	public class Selector<TChoice, TAbstractKey> : TextureObject2D, ISelectableList<TChoice>
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
        private bool IsRevising = false;

		public bool IsActive { get; set; }
		public int SelectedIndex { get; private set; }
		public IObservable<TChoice> OnSelectionChanged { get; set; }
		public IObservable<TChoice> OnMove { get; set; }
		public IObservable<TChoice> OnDecide { get; set; }
		public IObservable<TChoice> OnCancel { get; set; }
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
		public Func<Object2D, Vector2DF, CancellationToken, Task> SetCursorPosition { get; set; }
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

        public Selector(Controller<TAbstractKey> controller)
		{
			IsActive = false;
            IsDrawn = false;

			choiceSystem = new Choice<TAbstractKey>(0, controller);
			OnSelectionChanged = choiceSystem.OnSelectionChanged
                .Where(x => !IsRevising)
                .Select(e => choiceItems_[e].Choice);
			OnMove = choiceSystem.OnMove.Select(e => choiceItems_[e].Choice);
			OnDecide = choiceSystem.OnDecide.Select(e => choiceItems_[e].Choice);
			OnCancel = choiceSystem.OnCancel.Select(e => choiceItems_[e].Choice);

			choiceSystem.OnSelectionChanged.Subscribe(OnSelectionChangedHandler);
			SelectedIndex = choiceSystem.SelectedIndex;

            SetCursorPosition = async (o, p, c) =>
            {
                var completion = new TaskCompletionSource<Unit>();
                await UpdateManager.Instance.FrameUpdate
                  .Select(u => o.Position)
                  .EasingVector2DF(p, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10)
                  .Do(v => o.Position = v);
            };
        }

		public void AddChoice(TChoice choice, Object2D item)
		{
			choiceItems_.Add(new ChoiceItem(choice, item));
			choiceSystem.Size++;
		}

		public bool RemoveChoice(TChoice choice)
		{
			var index = choiceItems_.IndexOf(c => c.Choice.Equals(choice));
			if(index != -1)
			{
				choiceItems_.RemoveAt(index);
                if(index <= SelectedIndex)
                {
                    IsRevising = true;
                    choiceSystem.SelectedIndex--;
                    IsRevising = false;
                }
                choiceSystem.Size--;
                Console.WriteLine($"Removed, Current:{SelectedIndex}");
				return true;
			}
			else
			{
				return false;
			}
		}

		public void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel)
		{
			choiceSystem.BindKey(next, ChoiceControl.Next);
			choiceSystem.BindKey(prev, ChoiceControl.Previous);
			choiceSystem.BindKey(decide, ChoiceControl.Decide);
			choiceSystem.BindKey(cancel, ChoiceControl.Cancel);
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
                    MoveCursor(choiceItems_[index].Item.Position);
                }
			}
			else
			{
				IsDrawn = false;
			}

			SelectedIndex = choiceSystem.SelectedIndex;
		}

		private void MoveCursor(Vector2DF position)
		{
			cancellationOfCursorMoving?.Dispose();
			cancellationOfCursorMoving = SetCursorPosition(this, position + CursorOffset);
		}
	}
}
