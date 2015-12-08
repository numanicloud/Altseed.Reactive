﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
    public class CollectionBinderForSelector<TChoice> : IDisposable
    {
        private ISelector<TChoice> list { get; set; }
        private INotifyCollectionChanged notifier { get; set; }
        public Func<TChoice, Object2D> ChoiceToItem { get; set; }

        private CollectionBinderForSelector(ISelector<TChoice> list, INotifyCollectionChanged notifier)
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
                    list.Layer.AddObject(obj);
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(TChoice choice in e.OldItems)
                {
                    var item = list.RemoveChoice(choice);
                    item?.Vanish();
                }
            }
        }

        public void Dispose()
        {
            notifier.CollectionChanged -= Notifier_CollectionChanged;
        }

        public static IDisposable Bind(
            ISelector<TChoice> selector,
            ObservableCollection<TChoice> notifier,
            Func<TChoice, Object2D> choiceToItem,
			bool doInitialize)
        {
			if(doInitialize)
			{
				selector.ClearChoice();
				foreach(var choice in notifier)
				{
					var obj = choiceToItem(choice);
                    selector.AddChoice(choice, obj);
					selector.Layer.AddObject(obj);
				}
			}
            return new CollectionBinderForSelector<TChoice>(selector, notifier)
            {
                ChoiceToItem = choiceToItem
            };
        }
    }
}
