using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public abstract class Layouter : TextureObject2D
    {
        public abstract void AddItem(Object2D obj);
        public abstract void InsertItem(int index, Object2D obj);
        public abstract void RemoveItem(Object2D obj);
        public abstract void ClearItem();
        protected abstract IEnumerable<Object2D> ObjectsInternal { get; }

        protected override void OnUpdate()
        {
            var beVanished = new List<Object2D>();
            foreach(var item in ObjectsInternal)
            {
                if(!item.IsAlive)
                {
                    beVanished.Add(item);
                }
            }
            beVanished.ForEach(x => RemoveItem(x));
        }
    }
}
