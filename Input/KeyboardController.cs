using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	class KeyboardController<AbstractKey> : Controller<AbstractKey>
	{
		private Dictionary<AbstractKey, Keys> binding;

		public void BindKey(Keys key, AbstractKey abstractKey)
		{
			binding[abstractKey] = key;
		}

		public override KeyState GetState(AbstractKey key)
		{
			return Engine.Keyboard.GetKeyState(binding[key]);
		}
	}
}
