using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Object
{
	class ReactiveSceneTest
	{
		// メッセージ文字列のオブジェクトを生成する
		private asd.TextObject2D CreateMessage(string message, float y, asd.Font font)
		{
			return new asd.TextObject2D
			{
				Position = new asd.Vector2DF(10, y),
				Font = font,
				Text = message,
			};
		}

		// 指定したレイヤー全てにメッセージ文字列のオブジェクトを追加する
		private void AddMessageToLayers(string message, float y, asd.Font font, params asd.Layer2D[] layers)
		{
			foreach (var item in layers)
			{
				var obj = CreateMessage(message, y, font);
				item.AddObject(obj);
			}
		}

		public void Run()
		{
			asd.Engine.Initialize("ReactiveSceneTest", 640, 480, new asd.EngineOption());

			// ReactiveSceneクラスのインスタンスを作成する
			// 遷移前のシーン
			// レイヤーやオブジェクトも設定する
			var scene1 = new Altseed.Reactive.Object.ReactiveScene();
			var layer1 = new asd.Layer2D();
			var obj1 = new asd.TextureObject2D()
			{
				Position = new asd.Vector2DF(100, 100),
				Texture = asd.Engine.Graphics.CreateTexture2D("Resources/Scene1.png"),
			};
			scene1.AddLayer(layer1);
			layer1.AddObject(obj1);
			
			// 遷移先のシーン
			var scene2 = new Altseed.Reactive.Object.ReactiveScene();
			var layer2 = new asd.Layer2D();
			var obj2 = new asd.TextureObject2D()
			{
				Position = new asd.Vector2DF(100, 100),
				Texture = asd.Engine.Graphics.CreateTexture2D("Resources/Scene2.png"),
			};
			scene2.AddLayer(layer2);
			layer2.AddObject(obj2);

			// 遷移前のシーンを現在のシーンとして登録しておく
			asd.Engine.ChangeScene(scene1);

			// フォントを生成(メッセージ文字列を使うため)
			var font = asd.Engine.Graphics.CreateDynamicFont("", 32, new asd.Color(255, 255, 255), 0, new asd.Color(0, 0, 0));

			// 遷移前のシーンからの遷移が開始したら呼ばれる関数を登録する。
			// 関数内ではメッセージ文字列オブジェクトを生成してレイヤーに追加している
			scene1.OnTransitionBeginEvent
				.Subscribe(x => AddMessageToLayers("遷移開始", 10, font, layer1, layer2));

			// 遷移前のシーンの更新が停止したら呼ばれる関数を登録する。
			scene1.OnStopUpdatingEvent
				.Subscribe(x => AddMessageToLayers("更新停止", 40, font, layer1, layer2));

			// 遷移前のシーンがエンジンから登録解除されたら呼ばれる関数を登録する。
			scene1.OnUnregisteredEvent
				.Subscribe(x => AddMessageToLayers("登録解除", 70, font, layer1, layer2));

			// 遷移先のシーンがエンジンに登録されたら呼ばれる関数を登録する。
			scene2.OnRegisteredEvent
				.Subscribe(x => AddMessageToLayers("登録", 100, font, layer1, layer2));

			// 遷移先のシーンの更新が開始したら呼ばれる関数を登録する。
			scene2.OnStartUpdatingEvent
				.Subscribe(x => AddMessageToLayers("更新開始", 130, font, layer1, layer2));

			// 遷移先のシーンへの遷移が完了したら呼ばれる関数を登録する。
			scene2.OnTransitionFinishedEvent
				.Subscribe(x => AddMessageToLayers("遷移完了", 160, font, layer1, layer2));

			asd.Engine.CaptureScreenAsGifAnimation("Output/ReactiveSceneTest.gif", 90, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				if (asd.Engine.Keyboard.GetKeyState(asd.Keys.Enter) == asd.KeyState.Push)
				{
					asd.Engine.ChangeSceneWithTransition(scene2, new asd.TransitionFade(1, 1));
				}

				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
