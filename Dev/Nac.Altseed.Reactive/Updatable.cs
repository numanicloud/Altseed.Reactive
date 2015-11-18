using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive
{
    public class Updatable
    {
        private static Updatable instance_;

        public static Updatable Instance => instance_ ?? (instance_ = new Updatable());

        private Subject<float> frameUpdate;

        public IObservable<float> FrameUpdate => frameUpdate;

        private Updatable()
        {
        }

        public void Update()
        {
            frameUpdate.OnNext(Engine.DeltaTime);
        }
    }
}
