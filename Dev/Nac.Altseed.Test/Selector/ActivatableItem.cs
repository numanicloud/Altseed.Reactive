using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Selector
{
	class ActivatableItem : TextObject2D, IActivatableSelectionItem
	{
		public ActivatableItem(string title)
		{
			Font = asd.Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255), 0, new Color());
			Text = title;
			Color = new Color(255, 225, 32);
		}

		public void Activate()
		{
			Color = new Color(30, 30, 30);
		}

		public void Disactivate()
		{
			Color = new Color(255, 225, 32);
		}
	}
}
