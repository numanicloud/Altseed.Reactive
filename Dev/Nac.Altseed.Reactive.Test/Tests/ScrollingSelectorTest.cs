using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.UI;

namespace Nac.Altseed.Reactive.Test
{
	class ScrollingSelectorTest : AltseedTest
	{
		Font font;
		ScrollingSelector<int, Control> selector;

		protected override void OnStart()
		{
			selector = new ScrollingSelector<int, Control>(CreateController());
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector.IsControllerUpdated = true;
			selector.Position = new Vector2DF(30, 30);
			selector.LineSpan = 36;
			selector.BoundLines = 3;
			selector.LineWidth = 360;
			selector.ExtraLinesOnEnding = 0;
			selector.ExtraLinesOnStarting = 0;

			font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 0, 0, 255), 0, new Color(0, 0, 0, 0));

			var scene = new Scene();
			scene.AddLayer(selector);
			Engine.ChangeScene(scene);
			
			for(int i = 0; i < 10; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"アイテム{i}",
				};
				selector.AddChoice(i, obj);
				selector.Layer.AddObject(obj);
			}

			selector.SetDebugCameraUp();
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
			{
				Console.WriteLine("Remove");
				for(int i = 0; i < 6; i++)
				{
					selector.RemoveChoice(i);
				}
			}
		}
	}
}
