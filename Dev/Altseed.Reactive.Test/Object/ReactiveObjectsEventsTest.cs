using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Object
{
	class ReactiveObjectsEventsTest
	{
		public void Run()
		{
			asd.Engine.Initialize("ReactiveObjectsEventsTest", 640, 480, new asd.EngineOption());

			// ReactiveTextureObject2Dを生成
			var obj = new Altseed.Reactive.Object.ReactiveTextureObject2D();
			obj.Position = new asd.Vector2DF(10, 10);
			obj.Texture = asd.Engine.Graphics.CreateTexture2D("Resources/Heart.png");

			// このオブジェクトがUpdateされるときに呼ばれる関数を登録
			obj.OnUpdatedEvent
				.Subscribe(x => obj.Position += new asd.Vector2DF(1, 0));

			// このオブジェクトがレイヤーに追加されるときに呼ばれる関数を登録
			obj.OnAddedEvent
				.Subscribe(x => Console.WriteLine("Object added."));

			// このオブジェクトがレイヤーから削除されるときに呼ばれる関数を登録
			obj.OnRemovedEvent
				.Subscribe(x => Console.WriteLine("Object removed."));

			// このオブジェクトが破棄されるときに呼ばれる関数を登録
			obj.OnDisposeEvent
				.Subscribe(x => Console.WriteLine("Object disposed."));

			asd.Engine.AddObject2D(obj);	// レイヤーにオブジェクトを追加
			asd.Engine.RemoveObject2D(obj);	// レイヤーからオブジェクトを削除
			asd.Engine.AddObject2D(obj);	// レイヤーにオブジェクトを再度追加
			obj.Dispose();					// オブジェクトを破棄

			asd.Engine.CaptureScreenAsGifAnimation("Output/ReactiveObjectsEventsTest.gif", 60, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
