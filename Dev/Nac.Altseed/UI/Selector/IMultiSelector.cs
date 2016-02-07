using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.UI
{
	interface IMultiSelector<TChoice, TAbstractKey>
	{
		IObservable<TChoice> OnAdd { get; }
		IObservable<TChoice> OnRemove { get; }
		IObservable<TChoice> OnDecideForSingle { get; }
		IObservable<IEnumerable<Selector<TChoice, TAbstractKey>.ChoiceItem>> OnDecideForMulti { get; }

		void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel, TAbstractKey multi);
		void AddSelectedIndex();
		void RemoveSelectedIndex();
		void Refresh();
	}
}
