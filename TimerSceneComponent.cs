using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
    public class TimerSceneComponent : SceneComponent
    {
        private float time;
        private Action callback;

        public TimerSceneComponent(float time, Action callback)
        {
            this.time = time;
            this.callback = callback;
        }

        protected override void OnUpdated()
        {
            time -= Engine.DeltaTime;
            if (time <= 0)
            {
                callback();
                Vanish();
            }
        }
    }
}
