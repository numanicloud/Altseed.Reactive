using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
	class MultiSelectorTest : AltseedTest
	{
		class ListItem : TextObject2D, IActivatableSelectionItem
		{
			public ListItem()
			{
				Color = new Color(240, 196, 90, 255);
			}

			public void Activate()
			{
				Color = new Color(128, 0, 0, 255);
			}

			public void Disactivate()
			{
				Color = new Color(240, 196, 90, 255);
			}
		}

		MultiSelector<int, Control> selector;
		Font font;

		protected override void OnStart()
		{
			var scene = new ReactiveScene();
			var layer = new ReactiveLayer2D();

			var layout = new LinearPanel()
			{
				ItemSpan = new Vector2DF(0, 36),
				Position = new Vector2DF(20, 20),
			};
			layout.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
			Func<Object2D> cursorCreator = () => new TextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("ListCursor.png"),
				Color = new Color(0, 255, 0, 128),
			};

			selector = new MultiSelector<int, Control>(CreateController(), layout, cursorCreator)
			{
				Loop = true,
				CursorOffset = new Vector2DF(-5, -3),
			};
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel, Control.Sub);
			selector.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);

			font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
			for(int i = 0; i < 8; i++)
			{
				var obj = new ListItem()
				{
					Text = $"選択肢{i}",
					Font = font,
				};
				Engine.AddObject2D(obj);
				selector.AddChoice(i, obj);
			}

			var moveSound = Engine.Sound.CreateSoundSource("kachi38.wav", true);
			var decideSound = Engine.Sound.CreateSoundSource("pi78.wav", true);
			var cancelSound = Engine.Sound.CreateSoundSource("pi11.wav", true);

			selector.OnSelectionChanged.Subscribe(i => Engine.Sound.Play(moveSound));
			selector.OnDecide.Subscribe(i => Engine.Sound.Play(decideSound));
			selector.OnCancel.Subscribe(i => Engine.Sound.Play(cancelSound));
			selector.OnAdd.Subscribe(i => Engine.Sound.Play(decideSound));
			selector.OnRemove.Subscribe(i => Engine.Sound.Play(cancelSound));
			selector.OnDecideForMulti.Subscribe(xs =>
			{
				foreach(var x in xs.ToList())
				{
					selector.GetItemForChocie(x)?.Dispose();
					selector.RemoveChoice(x);
				}
			});

			Engine.ChangeScene(scene);
			scene.AddLayer(layer);
			layer.AddObject(selector);
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
			{
				var index = selector.ChoiceItems.Any() ? selector.ChoiceItems.Max(x => x.Choice) + 1 : 0;
				var obj = new ListItem()
				{
					Font = font,
					Text = $"追加アイテム{index}",
				};
				selector.AddChoice(index, obj);
			}
			else if(selector.ChoiceItems.Skip(3).Any() && Engine.Keyboard.GetKeyState(Keys.W) == KeyState.Push)
			{
				int index = selector.ChoiceItems.Max(x => x.Choice) + 1;
				var obj = new ListItem()
				{
					Font = font,
					Text = $"挿入アイテム{index}",
				};
				selector.InsertChoice(3, index, obj);
			}

			if(Engine.Keyboard.GetKeyState(Keys.LeftAlt) == KeyState.Hold)
			{
				if(Engine.Keyboard.GetKeyState(Keys.E) == KeyState.Push)
				{
					var item = selector.ChoiceItems.Skip(3).FirstOrDefault()?.Choice;
					if(item.HasValue)
					{
						selector.RemoveChoice(item.Value);
					}
				}
				else if(Engine.Keyboard.GetKeyState(Keys.R) == KeyState.Push)
				{
					selector.ClearChoice();
				}
			}
			else
			{
				if(Engine.Keyboard.GetKeyState(Keys.E) == KeyState.Push)
				{
					selector.ChoiceItems.Skip(3).FirstOrDefault()?.Item?.Dispose();
				}
				else if(Engine.Keyboard.GetKeyState(Keys.R) == KeyState.Push)
				{
					foreach(var item in selector.ChoiceItems)
					{
						item.Item.Dispose();
					}
				}
			}

			if(Engine.Keyboard.GetKeyState(Keys.Enter) == KeyState.Push)
			{
				selector.Dispose();
			}
		}
	}
}
