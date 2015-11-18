using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive
{
    /// <summary>
    /// Disposeメソッドによって動作の中断をするクラス。
    /// </summary>
    public class Cancelable : IDisposable
    {
        private IDisposable disposable;

        public Cancelable(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }

    public static class ReactiveAction
    {
        public static Cancelable SetEasingX(Object2D obj, float goal, EasingStart start, EasingEnd end, int count)
        {
            var disposable = Updatable.Instance.FrameUpdate
                .Select(x => obj.Position.X)
                .EasingValue(goal, start, end, count)
                .Subscribe(x => obj.Position = new Vector2DF(x, obj.Position.Y));
            return new Cancelable(disposable);
        }

        public static Cancelable SetShortWiggle(Object2D obj, Vector2DF center, Vector2DF amplitude, Vector2DF frequency, float time)
        {
            var disposable = Observable.ShortWiggle(center, amplitude, frequency, time)
                .Subscribe(v => obj.Position = v, () => obj.Position = center);
            return new Cancelable(disposable);
        }

        public static IDisposable TimeAnimation(Action<float> handler)
        {
            return Observable.CountTime().Subscribe(handler);
        }
    }
}
