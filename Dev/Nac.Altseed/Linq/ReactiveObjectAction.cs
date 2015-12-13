using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.Linq
{
    public static class ReactiveObjectAction
    {
        public static IDisposable StartEasingPosition<T>(this T obj, Vector2DF goal, EasingStart start, EasingEnd end, int count)
            where T : Object2D, INotifyUpdated
        {
            return obj.OnUpdateEvent.DoEasingPosition(obj, goal, start, end, count)
                .Subscribe();
        }

        public static IDisposable StartEasingX<T>(this T obj, float goal, EasingStart start, EasingEnd end, int count)
            where T : Object2D, INotifyUpdated
        {
            return obj.OnUpdateEvent.DoEasingX(obj, goal, start, end, count)
                .Subscribe();
        }

        public static IDisposable StartEasingY<T>(this T obj, float goal, EasingStart start, EasingEnd end, int count)
            where T : Object2D, INotifyUpdated
        {
            return obj.OnUpdateEvent.DoEasingY(obj, goal, start, end, count)
                .Subscribe();
        }


    }
}
