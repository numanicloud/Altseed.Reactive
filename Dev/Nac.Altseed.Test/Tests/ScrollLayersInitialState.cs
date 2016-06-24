using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test.Tests
{
	class ScrollLayersInitialState : AltseedTest
	{
		private ReactiveTextureObject2D obj;
		private IDisposable moving;
		private Subject<RectF> seeingAreaUpdate; 

		protected override void OnStart()
		{
			obj = new ReactiveTextureObject2D()
			{
				Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
			};
			seeingAreaUpdate = new Subject<RectF>();

			var scroll = new ScrollLayer()
			{
				CameraSize = new Vector2DF(640, 480),
				BoundaryStartingPosition = new Vector2DF(-180 - 320, -160 - 240),
				BoundaryEndingPosition = new Vector2DF(320 + 180, 240 + 160),
				BindingAreaRange = new RectF(0, 0, 640, 480),
			};
			scroll.SetEasingBehaviorUp(EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 15);
			scroll.SubscribeSeeingArea(seeingAreaUpdate);
			scroll.SetScrollPosition(new RectI(-320, -240, 640, 480), GetObjectArea());

			var scene = new Scene();
			scroll.AddObject(obj);
			scene.AddLayer(scroll);
			Engine.ChangeScene(scene);

			var rand = new Random();
			for (int i = 0; i < 60; i++)
			{
				int x = rand.Next()%1000 - 180 - 320;
				int y = rand.Next()%800 - 160 - 240;
				scroll.AddObject(new TextureObject2D
				{
					Texture = Engine.Graphics.CreateTexture2D("ZKey.png"),
					Position = new Vector2DF(x, y)
				});
			}
		}

		protected override void OnUpdate()
		{
			if (Engine.Keyboard.GetKeyState(Keys.Left) == KeyState.Push)
			{
				MoveObject(-100, 0);
			}
			else if (Engine.Keyboard.GetKeyState(Keys.Right) == KeyState.Push)
			{
				MoveObject(100, 0);
			}
			else if(Engine.Keyboard.GetKeyState(Keys.Up) == KeyState.Push)
			{
				MoveObject(0, -100);
			}
			else if(Engine.Keyboard.GetKeyState(Keys.Down) == KeyState.Push)
			{
				MoveObject(0, 100);
			}
		}

		private void MoveObject(int dx, int dy)
		{
			moving?.Dispose();
			if (dx != 0)
			{
				moving = obj.OnUpdateEvent.DoEasingX(obj, obj.Position.X + dx, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10)
					.Subscribe();
			}
			else
			{
				moving = obj.OnUpdateEvent.DoEasingY(obj, obj.Position.Y + dy, EasingStart.StartRapidly2, EasingEnd.EndSlowly3, 10)
					.Subscribe();
			}
			seeingAreaUpdate.OnNext(GetObjectArea().Shift(new Vector2DF(dx, dy)));
		}

		private RectF GetObjectArea()
		{
			return new RectF(obj.Position, new Vector2DF(96, 96));
		}
	}
}
