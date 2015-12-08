using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public interface ISelector<TChoice>
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
		void InsertChoice(int index, TChoice choice, Object2D obj);
		void ClearChoice();
    }

	public interface ISelector<TChoice, TAbstractKey> : ISelector<TChoice>
	{
		void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel);
	}
}
