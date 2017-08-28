using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI.ChoiceList;
using Nac.Altseed.UI.Cursor;
using System.Reactive.Linq;
using System.Reactive;
using Nac.Altseed.Linq;
using Nac.Altseed.Input;

namespace Nac.Altseed.UI
{
	public class SelectorFacade<TChoice, TControl>
	{
		public LinearPanel Layout { get; private set; }
		public EasingCursor Cursor { get; private set; }
		public LinearChoiceList<TChoice> ChoiceList { get; private set; }

		public Object2D SelectedItem => Layout.Items.ElementAt(ChoiceList.SelectedIndex).Object;

		public IObservable<int> OnSelectionChanged { get; }

		private Subject<Unit> ReviseTester { get; set; }

		public SelectorFacade(LinearPanel layout = null,
			EasingCursor cursor = null,
			LinearChoiceList<TChoice> choiceList = null)
		{
			Layout = layout ?? new LinearPanel();
			Cursor = cursor ?? new EasingCursor();
			ChoiceList = choiceList ?? new LinearChoiceList<TChoice>();
			ReviseTester = new Subject<Unit>();

			OnSelectionChanged = ReviseTester.Select(x => ChoiceList.SelectedIndex)
				.DistinctUntilChanged();

			OnSelectionChanged.Subscribe(x =>
				{
					Cursor.IsHidden = ChoiceList.SelectedIndex != ChoiceList.DisabledIndex;
					Cursor.MoveTo(Layout.Items.ElementAt(ChoiceList.SelectedIndex).Object);
				});
		}
		
		public void AddChoice(TChoice choice, Object2D obj)
		{
			Layout.AddItem(obj);
			ChoiceList.AddChoice(choice);
			ReviseTester.OnNext(Unit.Default);
		}

		public void InsertChoice(int index, TChoice choice, Object2D obj)
		{
			Layout.InsertItem(index, obj);
			ChoiceList.AddChoice(choice);
			ReviseTester.OnNext(Unit.Default);
		}

		public bool RemoveChoice(TChoice choice)
		{
			Layout.RemoveItem(SelectedItem);
			var result = ChoiceList.RemoveChoice(choice);

			ReviseTester.OnNext(Unit.Default);

			return result;
		}

		public void ClearChoice()
		{
			Layout.ClearItem();
			ChoiceList.ClearChoice();
			ReviseTester.OnNext(Unit.Default);
		}

		public bool SelectPreviousIndex()
		{
			var result = ChoiceList.SelectPreviousIndex();
			ReviseTester.OnNext(Unit.Default);
			return result;
		}

		public bool SelectNextIndex()
		{
			var result = ChoiceList.SelectNextIndex();
			ReviseTester.OnNext(Unit.Default);
			return result;
		}
	}
}
