using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
	class ScrollTest : AltseedTest
	{
		protected override void OnStart()
		{
			var layout = new LinearPanel()
			{
				ItemSpan = new Vector2DF(0, 36),
			};
			var selector = new Selector<int, Control>(CreateController(), layout);
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector.IsControllerUpdated = true;

			var font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 0, 0, 255), 0, new Color(0, 0, 0, 0));

			var size = selector.Cursor.Texture.Size;

			var scroll = new ScrollLayer()
			{
				Position = new Vector2DF(30, 30),
				CameraSize = new Vector2DF(200, 200),
				BindingAreaRange = new RectF(0, 25, 200, 150),
				Starting = new Vector2DF(0, 0),
				Ending = layout.ItemSpan * 10 + new Vector2DF(200, 0),
			};
			var areaChanged = selector.OnSelectionChanged
				.Select(c => selector.GetItemForChocie(c).Position)
				.Select(p => new RectF(p.X, p.Y, size.X, size.Y));
			scroll.SubscribeSeeingArea(areaChanged);

			var scene = new Scene();
			scene.AddLayer(scroll);
			selector.RegisterLayer(scroll);
			Engine.ChangeScene(scene);

			for(int i = 0; i < 10; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"アイテム{i}",
				};
				selector.AddChoice(i, obj);
				scroll.AddObject(obj);
			}

			var heart1 = new TextureObject2D()
			{
				Position = scroll.Ending - new Vector2DF(50, 50),
				Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
			};
			scroll.AddObject(heart1);
			var heart2 = new TextureObject2D()
			{
				Position = scroll.Starting - new Vector2DF(0, 50),
				Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
			};
			scroll.AddObject(heart2);

			var viewer = new ScrollBoundViewer(scroll);
			scroll.AddObject(viewer);
		}
	}
}
