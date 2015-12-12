using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.ObjectSystem
{
	public interface INotifyUpdated
	{
		IObservable<float> OnUpdateEvent { get; }
	}
}
