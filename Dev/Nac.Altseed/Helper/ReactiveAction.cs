using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
    /// <summary>
    /// Disposeメソッドによって動作の中断をするクラス。
    /// </summary>
    public class Cancelable : IDisposable
    {
        private IDisposable disposable;
        private Subject<Unit> onDispose_;

        public IObservable<Unit> OnDispose => onDispose_;

        public Cancelable(IDisposable disposable)
        {
            this.disposable = disposable;
            onDispose_ = new Subject<Unit>();
        }

        public void Dispose()
        {
            disposable.Dispose();
            onDispose_.OnNext(Unit.Default);
            onDispose_.OnCompleted();
        }
    }

    public static class ReactiveAction
    {
        public static Cancelable SetEasingX(this Object2D obj, float goal, EasingStart start, EasingEnd end, int count)
        {
            var disposable = UpdateManager.Instance.FrameUpdate
                .Select(x => obj.Position.X)
                .EasingValue(goal, start, end, count)
                .Subscribe(x => obj.Position = new Vector2DF(x, obj.Position.Y));
            return new Cancelable(disposable);
        }

		public static Cancelable SetEasing(this Object2D obj, Vector2DF goal, EasingStart start, EasingEnd end, int count)
		{
			var disposable = UpdateManager.Instance.FrameUpdate
				.Select(x => obj.Position)
				.EasingVector2DF(goal, start, end, count)
				.Subscribe(p => obj.Position = p);
			return new Cancelable(disposable);
		}

        public static Cancelable SetShortWiggle(this Object2D obj, Vector2DF center, Vector2DF amplitude, Vector2DF frequency, float time)
        {
            var disposable = ObservableHelper.ShortWiggle(center, amplitude, frequency, time)
                .Subscribe(v => obj.Position = v, () => obj.Position = center);
            var cancelable = new Cancelable(disposable);
            cancelable.OnDispose.Subscribe(u => obj.Position = center);
            return cancelable;
        }

        public static IDisposable TimeAnimation(Action<float> handler)
        {
            return ObservableHelper.CountTime().Subscribe(handler);
        }

		public static IDisposable RegisterUpdating(IUpdatable updatable)
		{
			return UpdateManager.Instance.FrameUpdate.Subscribe(f => updatable.Update());
		}
    }
}
