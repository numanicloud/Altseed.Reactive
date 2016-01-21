using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
    class ScrollingSelectorTest : AltseedTest
    {
        Font font;
        ScrollingSelector<int, Control> selector;

        protected override void OnStart()
        {
            selector = new ScrollingSelector<int, Control>(CreateController())
            {
                Position = new Vector2DF(30, 30),
                CursorOffset = new Vector2DF(-3, -4 + 18),
                LayoutStarting = new Vector2DF(0, 18),
                LineSpan = 36,
                LineWidth = 360,
                BoundLines = 2,
                ExtraLinesOnStarting = 1,
                ExtraLinesOnEnding = 1,
                IsControllerUpdated = true,
            };
            selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
            selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
            selector.Cursor.SetCenterPosition(CenterPosition.CenterLeft);
            selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);

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

            //selector.SetDebugCameraUp();
        }

        protected override void OnUpdate()
        {
            if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
            {
                selector.ChoiceItems.Skip(2).FirstOrDefault()?.Item.Vanish();
            }
        }
    }
}
