using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;

namespace Nac.Altseed.Reactive.UI
{
    public class Menu<TChoice, TAbstractKey> : TextureObject2D, ISelectableList<TChoice>
    {
        private Layouter layouter_ { get; set; }
        private Selector<TChoice, TAbstractKey> selector_ { get; set; }
        private INotifyCollectionChanged notifier { get; set; }

        public Layouter Layouter
        {
            get { return layouter_; }
            set
            {
                layouter_ = value;
                if(Selector != null)
                {
                    // 初期状態のSelectorだとエラーが出る
                    layouter_.AddChild(Selector, ChildMode.All);
                }
            }
        }
        public Selector<TChoice, TAbstractKey> Selector
        {
            get { return selector_; }
            set
            {
                selector_ = value;
                if(Layouter != null)
                {
                    Layouter.AddChild(selector_, ChildMode.All);
                }
            }
        }

        protected override void OnStart()
        {
            Layer.AddObject(Layouter);
            Layer.AddObject(Selector);
        }

        protected override void OnVanish()
        {
            Layouter.Vanish();
            Selector.Vanish();
        }

        public void AddChoice(TChoice choice, Object2D item)
        {
            Layouter.AddItem(item);
            Selector.AddChoice(choice, item);
        }

        public bool RemoveChoice(TChoice choice)
        {
            var obj = Selector.ChoiceItems.FirstOrDefault(x => x.Choice.Equals(choice))?.Item;
            if(obj != null)
            {
                Layouter.RemoveItem(obj);
                Selector.RemoveChoice(choice);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
