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
    /// <summary>
    /// Updateされたことを通知するイベントを持つシングルトン クラス。
    /// </summary>
    public class UpdateManager
    {
        private static UpdateManager instance_;

        /// <summary>
        /// このクラスのシングルトン オブジェクト。
        /// </summary>
        public static UpdateManager Instance => instance_ ?? (instance_ = new UpdateManager());

        private Subject<float> frameUpdate;

        /// <summary>
        /// Updateが呼ばれたときに発行されるイベント。
        /// </summary>
        public IObservable<float> FrameUpdate => frameUpdate;

        private UpdateManager()
        {
            frameUpdate = new Subject<float>();
        }

        /// <summary>
        /// FrameUpdateイベントを発行します。毎フレーム呼び出すことが推奨されます。
        /// </summary>
        public void Update()
        {
            frameUpdate.OnNext(Engine.DeltaTime);
        }
    }
}
