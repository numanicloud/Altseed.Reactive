using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Reactive.Input
{
    public static class ControllerExtention
    {
        public static IObservable<Unit> ObserveKeyState<TAbstractKey>(this Controller<TAbstractKey> controller, TAbstractKey key, InputState state)
        {
            return Updatable.Instance.FrameUpdate
                .Where(x => controller.GetState(key) == state)
                .Select(x => Unit.Default);
        }
    }
}
