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
                Position = new Vector2DF(60, 35),
                CursorOffset = new Vector2DF(-5, 3),
                LineSpan = 36,
                LineWidth = 360,
                BoundLines = 9,
                ExtraLinesOnStarting = 1,
                ExtraLinesOnEnding = 0,
                IsControllerUpdated = true,
				Loop = true,
            };
            selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
            selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
            selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
			selector.AddObject(new TextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("ListWindowLarge.png"),
				Position = new Vector2DF(30, 30),
				DrawingPriority = -1,
			});

            font = Engine.Graphics.CreateFont("MPlusB.aff");

            var scene = new Scene();

			var background = new Layer2D();
			background.AddObject(new GeometryObject2D()
			{
				Shape = new RectangleShape() { DrawingArea = new RectF(0, 0, 640, 480) },
				Color = new Color(255, 255, 255, 255),
			});
			scene.AddLayer(background);

			scene.AddLayer(selector);
			Engine.ChangeScene(scene);

			for(int i = 0; i < 15; i++)
            {
                var obj = new TextObject2D()
                {
                    Font = font,
                    Text = $"アイテム{i}",
					Color = new Color(225, 160, 0, 255),
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
                selector.ChoiceItems.Skip(2).FirstOrDefault()?.Item?.Dispose();
            }
        }
    }
}
