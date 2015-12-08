using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.UI;

namespace Nac.Altseed.Reactive.Test
{
	class BindCollectionForScrollingSelector : AltseedTest
	{
		Font font;
		ScrollingSelector<int, Control> selector;
		ObservableCollection<int> collection;

		protected override void OnStart()
		{
			font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 0, 0, 255), 0, new Color(0, 0, 0, 0));

			selector = new ScrollingSelector<int, Control>(CreateController())
			{
				IsControllerUpdated = true,
				Position = new Vector2DF(30, 30),
				LineSpan = 36,
				BoundLines = 4,
				LineWidth = 360,
				ExtraLinesOnStarting = 1,
				ExtraLinesOnEnding = 1,
			};
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);

			collection = new ObservableCollection<int>();

			for(int i = 0; i < 5; i++)
			{
				collection.Add(i);
			}

			CollectionBinderForSelector<int>.Bind(selector, collection, i => new TextObject2D()
			{
				Font = font,
				Text = $"アイテム{i}",
			}, true);

			for(int i = 5; i < 10; i++)
			{
				collection.Add(i);
			}

			var scene = new Scene();
			scene.AddLayer(selector);
			Engine.ChangeScene(scene);
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
			{
				var index = collection.Any() ? collection.Max() + 1 : 0;
				collection.Add(index);
			}
			else if(collection.Skip(2).Any() && Engine.Keyboard.GetKeyState(Keys.W) == KeyState.Push)
			{
				collection.RemoveAt(2);
			}
		}
	}
}
