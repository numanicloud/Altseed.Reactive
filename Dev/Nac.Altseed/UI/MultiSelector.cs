using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.Linq;

namespace Nac.Altseed.UI
{
	public class MultiSelector<TChoice, TAbstractKey> : Selector<TChoice, TAbstractKey>
	{
		class SelectionOfMultiSelection
		{
			public int Index { get; set; }
			public Object2D Cursor { get; set; }
		}

		private Subject<TChoice> onAddSubject_ = new Subject<TChoice>();
		private Subject<TChoice> onRemoveSubject_ = new Subject<TChoice>();
		private List<SelectionOfMultiSelection> selections;
		private Controller<TAbstractKey> controller;
		private IDisposable multiKeyBind;

		public Func<Object2D> CreateCursor { get; set; }
		public IObservable<TChoice> OnAdd => onAddSubject_;
		public IObservable<TChoice> OnRemove => onRemoveSubject_;
		public IObservable<TChoice> OnDecideForSingle { get; set; }
		public IObservable<IEnumerable<ChoiceItem>> OnDecideForMulti { get; set; }

		public MultiSelector(Controller<TAbstractKey> controller, Layouter layout)
			: base(controller, layout)
		{
			this.controller = controller;
			selections = new List<SelectionOfMultiSelection>();
			OnDecideForSingle = OnDecide.Where(x => !selections.Any());
			OnDecideForMulti = OnDecide.Where(x => selections.Any())
				.Select(x => selections.Select(y => ChoiceItems[y.Index]));
		}

		public void BindKey(TAbstractKey next, TAbstractKey previous, TAbstractKey decide, TAbstractKey cancel, TAbstractKey multi)
		{
			base.BindKey(next, previous, decide, cancel);
			multiKeyBind?.Dispose();
			multiKeyBind = OnUpdateEvent.Select(t => controller.GetState(multi))
				.Where(x => x == InputState.Push)
				.Subscribe(x =>
				{
					if(selections.Any(y => y.Index == SelectedIndex))
					{
						RemoveSelectedIndex();
					}
					else
					{
						AddSelectedIndex();
					}
				});
		}

		public void AddSelectedIndex()
		{
			var selection = new SelectionOfMultiSelection
			{
				Index = SelectedIndex,
				Cursor = CreateCursor(),
			};
			ChoiceItems[SelectedIndex].Item.AddChild(
				selection.Cursor,
				ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal,
				ChildTransformingMode.All);
			selections.Add(selection);
			onAddSubject_.OnNext(SelectedChoice);
		}

		public void RemoveSelectedIndex()
		{
			var selection = selections.First(x => x.Index == SelectedIndex);
			selection.Cursor.Dispose();
			selections.Remove(selection);
			onRemoveSubject_.OnNext(SelectedChoice);
		}

		public void Refresh()
		{
			selections.ForEach(x => x.Cursor.Dispose());
			selections.Clear();
		}

		public override void InsertChoice(int index, TChoice choice, Object2D item)
		{
			base.InsertChoice(index, choice, item);
			foreach(var s in selections)
			{
				if(index <= s.Index)
				{
					s.Index++;
				}
			}
		}

		public override Object2D RemoveChoice(TChoice choice)
		{
			var index = ChoiceItems.IndexOf(x => x.Choice.Equals(choice));
			var result = base.RemoveChoice(choice);
			if(index != -1)
			{
				foreach(var s in selections)
				{
					if(index < s.Index)
					{
						s.Index--;
					}
				}
				selections.RemoveAll(x => x.Index == index);
			}
			return result;
		}
	}
}
