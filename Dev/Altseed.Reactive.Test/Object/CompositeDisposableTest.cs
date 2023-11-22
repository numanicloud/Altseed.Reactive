using Altseed.Reactive.Object;
using asd;
using System;
using System.Reactive.Linq;

namespace Altseed.Reactive.Test.Object
{
	class CompositeDisposableTest
	{
		public void Run()
		{
			Engine.Initialize("CompositeDisposableTest", 640, 480, new EngineOption());
			
			// ReactiveTextureObject2D をインスタンス化する。
			var obj = new ReactiveTextureObject2D();
			obj.Texture = Engine.Graphics.CreateTexture2D("Resources/Heart.png");
			obj.Position = new Vector2DF(10, 10);
			Engine.AddObject2D(obj);

			var target = new TextureObject2D();
			target.Texture = Engine.Graphics.CreateTexture2D("Resources/Heart.png");
			target.Position = new Vector2DF(10, 100);
			Engine.AddObject2D(target);

			// 0.25秒ごとに target オブジェクトを右に移動する。
			// obj が破棄されると停止する。
			Observable.Interval(TimeSpan.FromSeconds(0.25))
				.Subscribe(x => target.Position += new Vector2DF(20, 0))
				.AddTo(obj);

			Engine.CaptureScreenAsGifAnimation("Output/CompositeDisposableTest.gif", 90, 0.5f, 0.5f);

			while (Engine.DoEvents())
			{
				// Enter を押すと obj オブジェクトを破棄
				if (Engine.Keyboard.GetKeyState(Keys.Enter) == ButtonState.Push)
				{
					obj.Dispose();
				}

				Engine.Update();
			}

			Engine.Terminate();
		}
	}
}
