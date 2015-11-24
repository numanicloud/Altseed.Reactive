using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
	class MessageWindowTest : AltseedTest
	{
		public MessageWindowTest()
			: base("MessageWindow")
		{
		}

		protected override void OnInitialize()
		{
			var controller = new KeyboardController<int>();
			controller.BindKey(asd.Keys.Z, 0);

			var window = new MessageWindow<int>(controller)
			{
				Texture = Engine.Graphics.CreateTexture2D("TextWindow.png"),
				Scale = new Vector2DF(0.5f, 1),
				ReadKey = 0,
			};
			window.TextObject.Font = Engine.Graphics.CreateDynamicFont("", 32, new Color(0, 0, 0, 25), 0, new Color(0, 0, 0, 0));
			window.TextObject.Position = new Vector2DF(30, 25);
			window.WaitIndicator.Texture = Engine.Graphics.CreateTexture2D("ZKey.png");
			window.WaitIndicator.Position = new Vector2DF(500, 110);

			Engine.AddObject2D(window);

			window.TalkMessage(new string[] { "あいうえお", "かきくけこ", "口から\n魚の骨が\nどんどん出てくる" }, null);
        }
	}
}
