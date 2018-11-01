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
	class ScrollingMultiSelectorTest : AltseedTest
	{
		ScrollingMultiSelector<int, Control> selector;

		protected override void OnStart()
		{
			Func<Object2D> cursorCreator = () => new TextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("ListCursor.png"),
				Color = new Color(0, 255, 0, 128),
			};

			selector = new ScrollingMultiSelector<int, Control>(CreateController(), cursorCreator)
			{
				Position = new asd.Vector2DF(60, 35),
				CursorOffset = new asd.Vector2DF(-5, 3),
				LineSpan = 36,
				LineWidth = 360,
				BoundLines = 9,
				ExtraLinesOnStarting = 1,
				ExtraLinesOnEnding = 0,
				IsControllerUpdated = true,
				Loop = true,
			};
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel, Control.Sub);
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
			selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);

			var font = Engine.Graphics.CreateFont("MPlusB.aff");

			var scene = new ReactiveScene();

			var background = new Layer2D();
			background.AddObject(new GeometryObject2D()
			{
				Shape = new RectangleShape() { DrawingArea = new RectF(0, 0, 640, 480) },
				Color = new Color(255, 255, 255, 255),
			});
			background.AddObject(new TextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("ListWindowLarge.png"),
				Position = new Vector2DF(30, 30),
				DrawingPriority = 1,
			});
			scene.AddLayer(background);

			scene.AddLayer(selector);
			Engine.ChangeScene(scene);

			for(int i = 0; i < 24; i++)
			{
				var obj = new TextObject2D()
				{
					Font = font,
					Text = $"アイテム{i}",
					Color = new Color(225, 160, 0, 255),
				};
				selector.AddChoice(i, obj);
			}

			var moveSound = Engine.Sound.CreateSoundSource("kachi38.wav", true);
			var decideSound = Engine.Sound.CreateSoundSource("pi78.wav", true);
			var cancelSound = Engine.Sound.CreateSoundSource("pi11.wav", true);

			selector.OnSelectionChanged.Subscribe(i =>
			{
				var handle = Engine.Sound.Play(moveSound);
				Engine.Sound.SetVolume(handle, 0.3f);
			});
			selector.OnDecide.Subscribe(i =>
			{
				var handle = Engine.Sound.Play(decideSound);
				Engine.Sound.SetVolume(handle, 0.3f);
			});
			selector.OnCancel.Subscribe(i => Engine.Sound.Play(cancelSound));
			selector.OnAdd.Subscribe(i => Engine.Sound.Play(decideSound));
			selector.OnRemove.Subscribe(i => Engine.Sound.Play(cancelSound));
			selector.OnDecideForMulti.Subscribe(xs =>
			{
				foreach(var x in xs.ToList())
				{
					selector.GetItemForChoice(x)?.Dispose();
					selector.RemoveChoice(x);
				}
			});

			//selector.SetDebugCameraUp();
		}

		protected override void OnUpdate()
		{
			if(Engine.Keyboard.GetKeyState(Keys.Q) == ButtonState.Push)
			{
				selector.ChoiceItems.Skip(2).FirstOrDefault()?.Item?.Dispose();
			}
		}
	}
}
