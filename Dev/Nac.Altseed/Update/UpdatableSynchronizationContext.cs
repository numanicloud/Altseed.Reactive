using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Nac.Altseed
{
    using TaskItem = Tuple<SendOrPostCallback, object>;

    /// <summary>
    /// 送られた継続処理を Update が呼ばれるたびに同一のスレッド上で実行する同期コンテキスト。
    /// </summary>
    public class UpdatableSynchronizationContext : SynchronizationContext
	{
		ConcurrentQueue<TaskItem> queue;

		public UpdatableSynchronizationContext()
		{
			queue = new ConcurrentQueue<TaskItem>();
		}

        /// <summary>
        /// この同期コンテキストに継続処理を送ります。
        /// </summary>
        /// <param name="d">継続処理。</param>
        /// <param name="state"></param>
		public override void Post(SendOrPostCallback d, object state)
		{
			if(d == null)
			{
				throw new ArgumentNullException();
			}
			queue.Enqueue(Tuple.Create(d, state));
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// 送られてきた継続処理を実行します。アプリケーションの実行中に定期的に呼び出すことが推奨されます。
        /// </summary>
		public void Update()
		{
			TaskItem item;
			while(queue.TryDequeue(out item))
			{
				item.Item1(item.Item2);
			}
        }
	}
}
