using System;

namespace Altseed.Reactive.Test.Object
{
	/// <summary>
	/// ハートが右に移動していくテスト。
	/// </summary>
	class ReactiveTextObject2DTest
	{
		public void Run()
		{
			asd.Engine.Initialize("ReactiveTextureObject2DTest", 640, 480, new asd.EngineOption());

			// ReactiveTextObject2Dを生成
			var obj = new Altseed.Reactive.Object.ReactiveTextObject2D();
			obj.Font = asd.Engine.Graphics.CreateDynamicFont("", 32, new asd.Color(255, 255, 255), 1, new asd.Color(0, 0, 0));
			obj.Position = new asd.Vector2DF(10, 10);
			obj.Text = "Text";

			// このオブジェクトがUpdateされるときに呼ばれる関数を登録
			// 関数内では、オブジェクトの位置を動かしている
			obj.OnUpdateEvent
				.Subscribe(x => obj.Position += new asd.Vector2DF(1, 0));

			asd.Engine.AddObject2D(obj);

			asd.Engine.CaptureScreenAsGifAnimation("Output/ReactiveTextureObject2DTest.gif", 60, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
