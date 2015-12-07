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
        void AddChoice(TChoice choice, Object2D obj);
        Object2D RemoveChoice(TChoice choice);
    }
}
