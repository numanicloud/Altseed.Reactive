using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public interface ISelectableList<TChoice>
    {
        Layer2D Layer { get; }

		bool IsActive { get; set; }
		int SelectedIndex { get; }
		IObservable<TChoice> OnSelectionChanged { get; }
		IObservable<TChoice> OnMove { get; }
		IObservable<TChoice> OnDecide { get; }
		IObservable<TChoice> OnCancel { get; }

		void AddChoice(TChoice choice, Object2D obj);
        Object2D RemoveChoice(TChoice choice);
    }

	public interface ISelectableList<TChoice, TAbstractKey> : ISelectableList<TChoice>
	{
		void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel);
	}
}
