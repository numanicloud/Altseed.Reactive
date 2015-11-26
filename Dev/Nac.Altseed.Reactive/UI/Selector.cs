using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
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

	public class Selector<TChoice, TAbstractKey> : IUpdatable
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
		private Cancelable cancellationOfCursorMoving = null;
		private Vector2DF cursorOffset_ = new Vector2DF();

		public bool IsActive { get; set; }
		public int SelectedIndex { get; private set; }
		public IObservable<TChoice> OnSelectionChanged { get; set; }
		public IObservable<TChoice> OnMove { get; set; }
		public IObservable<TChoice> OnDecide { get; set; }
		public IObservable<TChoice> OnCancel { get; set; }
		public IReadOnlyList<ChoiceItem> ChoiceItems => choiceItems_;

		public TextureObject2D Cursor { get; private set; }
		public Vector2DF CursorOffset
		{
			get { return cursorOffset_; }
			set
			{
				cursorOffset_ = value;
				if(choiceSystem.SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
				{
					cancellationOfCursorMoving?.Dispose();
					Cursor.Position = choiceItems_[choiceSystem.SelectedIndex].Item.Position + cursorOffset_;
				}
			}
		}
		public Func<Object2D, Vector2DF, Cancelable> SetCursorPosition { get; set; }

		public Selector(Controller<TAbstractKey> controller)
		{
			IsActive = false;
			Cursor = new TextureObject2D();
			Cursor.IsDrawn = false;

			choiceSystem = new Choice<TAbstractKey>(0, controller);
			OnSelectionChanged = choiceSystem.OnSelectionChanged.Select(e => choiceItems_[e].Choice);
			OnMove = choiceSystem.OnMove.Select(e => choiceItems_[e].Choice);
			OnDecide = choiceSystem.OnDecide.Select(e => choiceItems_[e].Choice);
			OnCancel = choiceSystem.OnCancel.Select(e => choiceItems_[e].Choice);

			choiceSystem.OnSelectionChanged.Subscribe(OnSelectionChangedHandler);
			SelectedIndex = choiceSystem.SelectedIndex;

			SetCursorPosition = (o, p) => Cursor.SetEasing(p, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
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
				var prev = choiceSystem.SelectedIndex;
				choiceItems_.RemoveAt(index);
				choiceSystem.Size--;
				if(index < choiceSystem.SelectedIndex)
				{
					choiceSystem.SelectedIndex--;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public void BindKey(TAbstractKey prev, TAbstractKey next, TAbstractKey decide, TAbstractKey cancel)
		{
			choiceSystem.BindKey(prev, ChoiceControl.Previous);
			choiceSystem.BindKey(next, ChoiceControl.Next);
			choiceSystem.BindKey(decide, ChoiceControl.Decide);
			choiceSystem.BindKey(cancel, ChoiceControl.Cancel);
		}

		public void Update()
		{
			if(IsActive)
			{
				choiceSystem.Update();
			}
		}


		private void OnSelectionChangedHandler(int index)
		{
			if(SelectedIndex != Choice<TAbstractKey>.DisabledIndex)
			{
				(choiceItems_[SelectedIndex].Item as IActivatableSelectionItem)?.Disactivate();
			}

			if(index != Choice<TAbstractKey>.DisabledIndex)
			{
				Cursor.IsDrawn = true;
				(choiceItems_[index].Item as IActivatableSelectionItem)?.Activate();
				MoveCursor(choiceItems_[index].Item.Position);
			}
			else
			{
				Cursor.IsDrawn = false;
			}

			SelectedIndex = choiceSystem.SelectedIndex;
		}

		private void MoveCursor(Vector2DF position)
		{
			cancellationOfCursorMoving?.Dispose();
			cancellationOfCursorMoving = SetCursorPosition(Cursor, position + CursorOffset);
		}
	}
}
