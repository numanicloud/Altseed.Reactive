using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Selector
{
	class ScrollingSelectorViewingOutside : AltseedTest
	{
		protected override void OnStart()
		{
			var selector = new ScrollingSelector<int, Control>(CreateController())
			{
				Position = new Vector2DF(60, 32),
				CursorOffset = new Vector2DF(-5, 3),
				LineSpan = 36,
				LineWidth = 360,
				BoundLines = 2,
				ExtraLinesOnStarting = 0,
				ExtraLinesOnEnding = 0,
				IsControllerUpdated = true,
				ScrollIntoVoid = true,
			};
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");

			var font = Engine.Graphics.CreateFont("MPlusB.aff");

			var scene = new ReactiveScene();

			var background = new Layer2D();
			background.AddObject(new GeometryObject2D()
			{
				Shape = new RectangleShape() { DrawingArea = new RectF(0, 0, 640, 480) },
				Color = new Color(255, 255, 255, 255),
			});
			scene.AddLayer(background);

			scene.AddLayer(selector);
			Engine.ChangeScene(scene);

			for(int i = 0; i < 10; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"アイテム{i}",
					Color = new Color(225, 160, 0, 255),
				};
				selector.AddChoice(i, obj);
			}

			selector.IsActive = true;
			selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
		}
	}
}
