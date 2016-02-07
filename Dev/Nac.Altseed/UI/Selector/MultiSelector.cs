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
	public class MultiSelector<TChoice, TAbstractKey>
		: Selector<TChoice, TAbstractKey>, IMultiSelector<TChoice, TAbstractKey>, ISelector<TChoice, TAbstractKey>
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
		private Func<Object2D> createCursor { get; set; }

		public IObservable<TChoice> OnAdd => onAddSubject_;
		public IObservable<TChoice> OnRemove => onRemoveSubject_;
		public IObservable<TChoice> OnDecideForSingle { get; private set; }
		public IObservable<IEnumerable<ChoiceItem>> OnDecideForMulti { get; private set; }

		public MultiSelector(Controller<TAbstractKey> controller, Layouter layout, Func<Object2D> createCursor)
			: base(controller, layout)
		{
			this.controller = controller;
			selections = new List<SelectionOfMultiSelection>();
			OnDecideForSingle = OnDecide.Where(x => !selections.Any());
			OnDecideForMulti = OnDecide.Where(x => selections.Any())
				.Select(x => selections.Select(y => ChoiceItems[y.Index]));
			this.createCursor = createCursor;
		}
		
		public void BindKey(TAbstractKey next,
			TAbstractKey prev,
			TAbstractKey decide,
			TAbstractKey cancel,
			TAbstractKey multi)
		{
			base.BindKey(next, prev, decide, cancel);
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
				Cursor = createCursor(),
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

		public override void InsertChoice(int index, TChoice choice, Object2D obj)
		{
			base.InsertChoice(index, choice, obj);
			foreach(var s in selections)
			{
				if(index <= s.Index)
				{
					s.Index++;
				}
			}
		}

		void ISelector<TChoice, TAbstractKey>.BindKey(TAbstractKey next,
			TAbstractKey prev,
			TAbstractKey decide,
			TAbstractKey cancel)
		{
			base.BindKey(next, prev, decide, cancel);
		}
	}
}
