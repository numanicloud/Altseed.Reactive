using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Selector
{
	class SelectorForSelector : AltseedTest
	{
		protected override void OnStart()
		{
			var cursorTexture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			var font = Engine.Graphics.CreateFont("MPlusB.aff");

			Func<LinearPanel> createLayouter = () => new LinearPanel()
			{
				ItemSpan = new Vector2DF(0, 40)
			};
			Selector<int, Control>[] selectors = Enumerable.Range(0, 2)
				.Select(x => new Selector<int, Control>(CreateController(), createLayouter()))
				.ToArray();

			var topLayouter = new LinearPanel()
			{
				ItemSpan = new Vector2DF(300, 0)
			};
			var topSelector = new Selector<int, Control>(CreateController(), topLayouter);
			topSelector.BindKey(Control.Right, Control.Left, Control.Decide, Control.Cancel);
			topSelector.IsActive = true;

			topSelector.OnSelectionChanged.Subscribe(x =>
			{
				foreach (var selector in selectors)
				{
					selector.IsActive = false;
					selector.HideSelection = true;
				}
				selectors[x].IsActive = true;
				selectors[x].HideSelection = false;
			});

			for (int j = 0; j < selectors.Length; j++)
			{
				selectors[j].BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
				selectors[j].Cursor.Texture = cursorTexture;
				for(int i = 0; i < 5; i++)
				{
					selectors[j].AddChoice(i, new TextObject2D()
					{
						Text = $"選択肢{i}",
						Font = font,
					});
				}
				selectors[j].OnSelectionChanged.Subscribe(x =>
				{
					foreach (var selector in selectors)
					{
						selector.ChangeSelection(x);
					}
				});
				topSelector.AddChoice(j, selectors[j]);
			}

			Engine.AddObject2D(topSelector);
		}
	}
}
