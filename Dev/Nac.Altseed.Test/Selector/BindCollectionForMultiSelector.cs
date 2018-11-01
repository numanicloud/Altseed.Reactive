using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Selector
{
	class BindCollectionForMultiSelector : AltseedTest
	{
		Font font;
		MultiSelector<int, Control> selector;
		ObservableCollection<int> collection;

		protected override void OnStart()
		{
			font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 0, 0, 255), 0, new Color(0, 0, 0, 0));

			var layout = new LinearPanel()
			{
				ItemSpan = new Vector2DF(0, 40),
			};

			var texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector = new MultiSelector<int, Control>(
				CreateController(),
				layout,
				() => new TextureObject2D
				{
					Texture = texture,
					Color = new Color(0, 255, 0, 128),
				})
			{
				IsControllerUpdated = true,
				Position = new Vector2DF(30, 30),
			};
			
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel, Control.Sub);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			layout.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
			selector.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
			selector.OnDecideForMulti.Subscribe(xs =>
			{
				foreach(var item in xs.ToArray())
				{
					collection.Remove(item);
				}
			});

			collection = new ObservableCollection<int>();

			for(int i = 0; i < 5; i++)
			{
				collection.Add(i);
			}

			CollectionBinderForSelector<int>.Bind(selector, collection, i => new TextObject2D()
			{
				Font = font,
				Text = $"アイテム{i}",
				Color = new Color(0, 0, 0, 255),
			}, true);

			for(int i = 5; i < 10; i++)
			{
				collection.Add(i);
			}

			Engine.AddObject2D(new GeometryObject2D
			{
				Shape = new RectangleShape { DrawingArea = new RectF(0, 0, 640, 480) },
				DrawingPriority = -1
			});
			Engine.AddObject2D(selector);
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == ButtonState.Push)
			{
				var index = collection.Any() ? collection.Max() + 1 : 0;
				collection.Add(index);
			}
			else if(collection.Skip(2).Any() && Engine.Keyboard.GetKeyState(Keys.W) == ButtonState.Push)
			{
				collection.RemoveAt(2);
			}
		}
	}
}
