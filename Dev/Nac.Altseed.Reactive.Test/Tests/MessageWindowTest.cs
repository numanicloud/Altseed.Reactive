using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;
using Nac.Altseed.Reactive.UI;

namespace Nac.Altseed.Test.ReactiveTests
{
    class MessageWindowTest : AltseedTest
    {
        public MessageWindowTest() : base("MessageWindow")
        {
        }

        protected override async void OnInitialize()
        {
            var controller = new KeyboardController<int>();
            controller.BindKey(Keys.Z, 0);

            var window = new MessageWindow()
            {
                Texture = Engine.Graphics.CreateTexture2D("TextWindow.png"),
                Scale = new Vector2DF(0.5f, 1),
            };
            window.TextObject.Font = Engine.Graphics.CreateDynamicFont("", 32, new Color(0, 0, 0, 255), 0, new Color(0, 0, 0, 0));
            window.TextObject.Position = new Vector2DF(30, 25);
            window.WaitIndicator.Texture = Engine.Graphics.CreateTexture2D("ZKey.png");
            window.WaitIndicator.Position = new Vector2DF(500, 110);
            window.SetReadControl(controller, 0);

            Engine.AddObject2D(window);

            await window.TalkMessageAsync(new string[] { "あいうえお", "かきくけこ", "口から\n魚の骨が\nどんどん出てくる" });
            window.Clear();
        }
    }
}
