using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Study
{
	class DisposeChild : AltseedTest
	{
		LinearPanel layout;
		TextureObject2D item;
		TextureObject2D parent;
		TextureObject2D child;

		protected override void OnStart()
		{
			layout = new LinearPanel()
			{
				ItemSpan = new asd.Vector2DF(0, 30),
			};
			layout.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly2, 10);

			Texture2D texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			item = new TextureObject2D()
			{
				Texture = texture
			};
			parent = new TextureObject2D()
			{
				Texture = texture
			};
			child = new TextureObject2D()
			{
				Texture = texture
			};

			layout.AddItem(item);
			layout.AddItem(parent);
			parent.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);

			Engine.AddObject2D(layout);
			Engine.AddObject2D(child);
		}

		protected override void OnUpdate()
		{
			if(TimeCount == 60)
			{
				child.Dispose();
				layout.RemoveItem(item);
			}
		}
	}
}
