using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public class CollectionBinderForSelector<TChoice> : IDisposable
    {
        private ISelectableList<TChoice> list { get; set; }
        private INotifyCollectionChanged notifier { get; set; }
        public Func<TChoice, Object2D> ChoiceToItem { get; set; }

        private CollectionBinderForSelector(ISelectableList<TChoice> list, INotifyCollectionChanged notifier)
        {
            this.list = list;
            this.notifier = notifier;
            notifier.CollectionChanged += Notifier_CollectionChanged;
            ChoiceToItem = c => new TextureObject2D();
        }

        private void Notifier_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(TChoice choice in e.NewItems)
                {
                    var obj = ChoiceToItem(choice);
                    list.AddChoice(choice, obj);
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(TChoice choice in e.OldItems)
                {
                    list.RemoveChoice(choice);
                }
            }
        }

        public void Dispose()
        {
            notifier.CollectionChanged -= Notifier_CollectionChanged;
        }

        public static IDisposable Bind(ISelectableList<TChoice> list, INotifyCollectionChanged notifier)
        {
            return new CollectionBinderForSelector<TChoice>(list, notifier);
        }
    }
}
