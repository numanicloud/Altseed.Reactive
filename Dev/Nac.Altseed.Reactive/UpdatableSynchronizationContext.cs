using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Nac.Altseed.Reactive
{
    using TaskItem = Tuple<SendOrPostCallback, object>;

    public class UpdatableSynchronizationContext : SynchronizationContext
	{
		ConcurrentQueue<TaskItem> queue;

		public UpdatableSynchronizationContext()
		{
			queue = new ConcurrentQueue<TaskItem>();
		}

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
