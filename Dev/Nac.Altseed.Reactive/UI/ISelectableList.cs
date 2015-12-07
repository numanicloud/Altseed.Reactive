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
        void AddChoice(TChoice choice, Object2D obj);
        bool RemoveChoice(TChoice choice);
    }
}
