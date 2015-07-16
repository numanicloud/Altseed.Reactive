using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	abstract class Controller<AbstractKey>
	{
		public abstract KeyState GetState(AbstractKey key);
	}
}
