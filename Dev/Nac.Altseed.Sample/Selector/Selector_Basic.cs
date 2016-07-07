using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.UI;
using Nac.Altseed.UI.Selector;

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

			var selector = new SimpleSelector<int>();
			selector.ItemSpan = new Vector2DF(0, 40);
			selector.Cursor.Texture = asd.Engine.Graphics.CreateTexture2D("Data/Selector/ListCursor.png");
			selector.BindKey(Keys.Down, Keys.Up, Keys.Z, Keys.X);
			Engine.AddObject2D(selector);

			var font = Engine.Graphics.CreateDynamicFont("", 28, new Color(255, 0, 0), 0, new Color(255, 255, 255));
			for (int i = 0; i < 9; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"選択肢{i}",
				};
				selector.AddChoice(i, obj);
			}
			
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
