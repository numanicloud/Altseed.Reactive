using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
	class AsIsLayoutTest : AltseedTest
	{
		LinearPanel linearPanel;
		AsIsLayout layout;
		Font font;

		protected override void OnStart()
		{
			layout = new AsIsLayout();
			linearPanel = new LinearPanel()
			{
				StartingOffset = new Vector2DF(10, 10),
				ItemSpan = new Vector2DF(5, 20),
			};
			Engine.AddObject2D(layout);
			Engine.AddObject2D(linearPanel);

			font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
			for(int i = 0; i < 7; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"選択肢{i}",
				};
				layout.AddItem(obj);
				linearPanel.AddItem(obj);
			}
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = "追加アイテム",
				};
				layout.AddItem(obj);
				linearPanel.AddItem(obj);
			}
			else if(Engine.Keyboard.GetKeyState(Keys.W) == KeyState.Push && layout.Items.Skip(3).Any())
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = "挿入アイテム",
				};
				layout.InsertItem(3, obj);
				linearPanel.InsertItem(3, obj);
			}

			if(Engine.Keyboard.GetKeyState(Keys.LeftAlt) == KeyState.Hold)
			{
				if(Engine.Keyboard.GetKeyState(Keys.E) == KeyState.Push)
				{
					var item = layout.Items.Skip(2).FirstOrDefault();
					if(item != null)
					{
						layout.RemoveItem(item);
						linearPanel.RemoveItem(item);
					}
				}
				else if(Engine.Keyboard.GetKeyState(Keys.R) == KeyState.Push)
				{
					layout.ClearItem();
					linearPanel.ClearItem();
				}
			}
			else
			{
				if(Engine.Keyboard.GetKeyState(Keys.E) == KeyState.Push)
				{
					layout.Items.Skip(2).FirstOrDefault()?.Dispose();
				}
				else if(Engine.Keyboard.GetKeyState(Keys.R) == KeyState.Push)
				{
					foreach(var item in layout.Items)
					{
						item.Dispose();
					}
				}
			}
		}
	}
}
