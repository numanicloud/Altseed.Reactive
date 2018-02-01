using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Object
{
	class ReactiveLayer2DTest
	{
		public void Run()
		{
			asd.Engine.Initialize("ReactiveLayer2DTest", 640, 480, new asd.EngineOption());

			// シーン・レイヤー・オブジェクトを設定
			var scene = new asd.Scene();
			// ReactiveLayer2D をインスタンス化
			var layer = new Altseed.Reactive.Object.ReactiveLayer2D();
			var obj = new asd.TextureObject2D()
			{
				Texture = asd.Engine.Graphics.CreateTexture2D("Resources/Heart.png"),
				Position = new asd.Vector2DF(10, 10),
			};
			asd.Engine.ChangeScene(scene);
			scene.AddLayer(layer);
			layer.AddObject(obj);

			// レイヤーが更新される直前に実行する関数を登録する。
			layer.OnUpdatingEvent
				.Subscribe(x => obj.Position += new asd.Vector2DF(0, 1));

			// レイヤーが更新された直後に実行する関数を登録する。
			layer.OnUpdatedEvent
				.Subscribe(x => obj.Position += new asd.Vector2DF(1, 0));

			asd.Engine.CaptureScreenAsGifAnimation("Output/ReactiveLayer2DTest.gif", 60, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
