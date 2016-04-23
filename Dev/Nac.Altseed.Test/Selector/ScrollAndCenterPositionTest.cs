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
	class ScrollAndCenterPositionTest : AltseedTest
	{
		class HomeItem : ReactiveTextureObject2D, IActivatableSelectionItem
		{
			private TextureObject2D child;

			public HomeItem(int index)
			{
				Texture = Engine.Graphics.CreateTexture2D("HomeMenuItem.png");
				Color = new Color(30, 30, 30);

				child = new TextureObject2D()
				{
					Texture = Engine.Graphics.CreateTexture2D($"ShopChoice{index}.png"),
				};
				this.SetCenterPosition(Altseed.CenterPosition.CenterCenter);
				child.SetCenterPosition(Altseed.CenterPosition.CenterCenter);

				AddChild(child, ChildManagementMode.RegistrationToLayer, ChildTransformingMode.Position);
			}

			public void Activate()
			{
				Color = new Color(255, 225, 0);
				child.Color = new Color(0, 0, 0);
			}

			public void Disactivate()
			{
				Color = new Color(30, 30, 30);
				child.Color = new Color(255, 255, 255);
			}
		}
		
		ScrollingSelector<int, Control> selector;

		protected override void OnStart()
		{
			selector = new ScrollingSelector<int, Control>(CreateController())
			{
				Position = new Vector2DF(320 - 200, 60),
				LineSpan = 52,
				LineWidth = 400,
				BoundLines = 4,
				IsControllerUpdated = true,
				Loop = true,
			};
			selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
			selector.Cursor.IsDrawn = false;

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

			for(int i = 0; i < 1; i++)
			{
				selector.AddChoice(i, new HomeItem(i));
			}

			selector.SetDebugCameraUp();
			selector.SetEasingScrollUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10);
		}

		protected override void OnUpdate()
		{
			if (Engine.Keyboard.GetKeyState(Keys.X) == KeyState.Push)
			{
				selector.ChangeSelection(3);
			}
		}
	}
}
