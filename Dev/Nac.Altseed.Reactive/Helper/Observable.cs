using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive
{
    public static class ObservableHelper
    {
        public static IObservable<U> SelectCorourine<T, U>(this IObservable<T> source, IEnumerator<U> coroutine)
        {
            return source.TakeWhile(x => coroutine.MoveNext())
                .Select(x => coroutine.Current);
        }

        public static IObservable<T> FromCoroutine<T>(IEnumerator<T> coroutine)
        {
            return UpdateManager.Instance.FrameUpdate
                .TakeWhile(x => coroutine.MoveNext())
                .Select(x => coroutine.Current);
        }

        public static IObservable<float> EasingValue(this IObservable<float> source, float goal, EasingStart start, EasingEnd end, int count)
        {
            var f = Easing.GetEasingFunction(start, end);
            return source.Select((x, i) => Easing.GetNextValue(x, goal, i, count, f))
                .Take(count + 1);
        }

        public static IObservable<Vector2DF> EasingVector2DF(this IObservable<Vector2DF> source, Vector2DF goal, EasingStart start, EasingEnd end, int count)
        {
            var f = Easing.GetEasingFunction(start, end);
            return source.Select((v, i) => Easing.GetNextValue(v, goal, i, count, f))
                .Take(count + 1);
        }

        public static IObservable<float> CountTime()
        {
            float time = 0;
            return UpdateManager.Instance.FrameUpdate
                .Do(f => time += f)
                .Select(x => time);
        }

        public static IObservable<int> CountFrame()
        {
            int frame = 0;
            return UpdateManager.Instance.FrameUpdate
                .Do(f => frame++)
                .Select(x => frame);
        }

        public static IObservable<Vector2DF> ShortWiggle(Vector2DF center, Vector2DF amplitude, Vector2DF frequency, float time)
        {
            return CountTime()
                .TakeWhile(t => t < time)
                .Select(t => new
                {
                    Phase = t * Math.PI * 2,
                    Height = 1 - (t / time)
                })
                .Select(p =>
                {
                    var x = (float)Math.Sin(p.Phase * frequency.X) * amplitude.X * p.Height;
                    var y = (float)Math.Sin(p.Phase * frequency.Y) * amplitude.Y * p.Height;
                    return center + new Vector2DF(x, y);
                });
        }
    }
}
