using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.UI;

namespace Nac.Altseed.Sample.Selector
{
	class Selector_Basic : ISample
	{
		enum Control
		{
			Down, Up, Decide, Cancel
		}

		public void Run()
		{
			asd.Engine.Initialize("Selector_Basic", 640, 480, new asd.EngineOption());

			var controller = new KeyboardController<Control>();
			controller.BindKey(Keys.Down, Control.Down);
			controller.BindKey(Keys.Up, Control.Up);
			controller.BindKey(Keys.Z, Control.Decide);
			controller.BindKey(Keys.X, Control.Cancel);

			var linearPanel = new LinearPanel()
			{
				ItemSpan = new Vector2DF(0, 40)
			};

			var selector = new Selector<int, Control>(controller, linearPanel);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("Data/Selector/ListCursor.png");
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.IsActive = true;

			var font = Engine.Graphics.CreateDynamicFont("", 28, new Color(255, 0, 0), 1, new Color(255, 255, 255));
			for (int i = 0; i < 9; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"選択肢{i}",
				};
				selector.AddChoice(i, obj);
			}

			Engine.AddObject2D(selector);
			
			while(asd.Engine.DoEvents())
			{
				asd.Engine.Update();
				Recorder.TakeScreenShot("Selector_Basic", 20);
			}
			
			asd.Engine.Terminate();
		}

		public string Description => "セレクターの使い方のサンプルです。";
		public string Title => "セレクターの基本";
	}
}
