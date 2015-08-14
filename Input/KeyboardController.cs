using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	public class KeyboardController<TAbstractKey> : Controller<TAbstractKey>
	{
		private Dictionary<TAbstractKey, Keys> binding;

		public override IEnumerable<TAbstractKey> Keys
		{
			get { return binding.Keys; }
		}

		public KeyboardController()
		{
			binding = new Dictionary<TAbstractKey, Keys>();
		}

		public void BindKey(Keys key, TAbstractKey abstractKey)
		{
			binding[abstractKey] = key;
		}

		public void BindDirection(TAbstractKey left, TAbstractKey right, TAbstractKey up, TAbstractKey down)
		{
			binding[left] = asd.Keys.Left;
			binding[right] = asd.Keys.Right;
			binding[up] = asd.Keys.Up;
			binding[down] = asd.Keys.Down;
		}

		public override InputState? GetState(TAbstractKey key)
		{
			if (binding.ContainsKey(key))
			{
				return (InputState)Engine.Keyboard.GetKeyState(binding[key]);
			}
			else
			{
				return null;
			}
		}
	}
}
