using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Object
{
	class ReactiveGeometryObject2DTest
	{
		public void Run()
		{
			asd.Engine.Initialize("ReactiveTextureObject2DTest", 640, 480, new asd.EngineOption());

			// ReactiveGeometryObject2Dを生成
			var obj = new Altseed.Reactive.Object.ReactiveGeometryObject2D();
			obj.Position = new asd.Vector2DF(10, 10);
			obj.Shape = new asd.RectangleShape()
			{
				DrawingArea = new asd.RectF(0, 0, 50, 100),
			};

			// このオブジェクトがUpdateされるときに呼ばれる関数を登録
			// 関数内では、オブジェクトの位置を動かしている
			obj.OnUpdatedEvent
				.Subscribe(x => obj.Position += new asd.Vector2DF(1, 0));

			asd.Engine.AddObject2D(obj);

			asd.Engine.CaptureScreenAsGifAnimation("Output/ReactiveGeometryObject2DTest.gif", 60, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
