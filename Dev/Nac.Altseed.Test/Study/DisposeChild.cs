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
		TextureObject2D parent;
		TextureObject2D child;

		protected override void OnStart()
		{
			Texture2D texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			parent = new TextureObject2D()
			{
				Texture = texture
			};
			child = new TextureObject2D()
			{
				Texture = texture
			};
			
			parent.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);

			Engine.AddObject2D(parent);
			Engine.AddObject2D(child);
		}

		protected override void OnUpdate()
		{
			if(TimeCount == 60)
			{
				child.Dispose();
			}
			if(TimeCount == 65)
			{
				parent.Position += new Vector2DF(10, 0);
			}
		}
	}
}
