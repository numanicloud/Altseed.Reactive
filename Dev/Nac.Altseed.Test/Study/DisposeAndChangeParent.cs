using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Study
{
	class DisposeAndChangeParent : AltseedTest
	{
		TextureObject2D item;
		TextureObject2D parent, parent2, parent3;
		TextureObject2D child;

		protected override void OnStart()
		{
			Texture2D texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			item = new TextureObject2D()
			{
				Texture = texture,
				Position = new Vector2DF(0, 0),
				Color = new Color(255, 0, 0),
			};
			parent = new TextureObject2D()
			{
				Texture = texture,
				Position = new Vector2DF(0, 40),
				Color = new Color(0, 255, 0),
			};
			parent2 = new TextureObject2D()
			{
				Texture = texture,
				Position = new Vector2DF(0, 80),
				Color = new Color(128, 128, 128),
			};
			parent3 = new TextureObject2D()
			{
				Texture = texture,
				Position = new Vector2DF(0, 120),
				Color = new Color(128, 0, 0),
			};
			child = new TextureObject2D()
			{
				Texture = texture,
				Position = new Vector2DF(450, 0),
			};
			
			parent.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);

			Engine.AddObject2D(item);
			Engine.AddObject2D(parent);
			Engine.AddObject2D(parent2);
			Engine.AddObject2D(parent3);
			Engine.AddObject2D(child);
		}

		protected override void OnUpdate()
		{
			if(TimeCount == 60)
			{
				parent.Dispose();
				parent2.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);
			}
			if(TimeCount == 120)
			{
				child.Parent?.RemoveChild(child);
				parent3.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);
			}
			if(TimeCount == 180)
			{
				child.Parent?.RemoveChild(child);
				parent2.AddChild(child, ChildManagementMode.Nothing, ChildTransformingMode.Position);
			}
		}
	}
}
