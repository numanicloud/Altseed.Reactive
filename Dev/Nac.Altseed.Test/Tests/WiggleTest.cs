using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Object2DComponents;

namespace Nac.Altseed.Test
{
	class WiggleTest : AltseedTest
	{
		Vector2DF center;
		TextureObject2D obj;

		public WiggleTest() : base("Wiggle")
		{
		}

		protected override void OnInitialize()
		{
			center = new Vector2DF(320, 240);
            obj = new TextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
				Position = center,
			};
			Engine.AddObject2D(obj);
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Z) == KeyState.Push)
			{
				obj.AddComponent(new ShortWiggleComponent(center, new Vector2DF(10, 10), new Vector2DF(10, 7), 3), "Wiggle"); 
			}
		}
	}
}
