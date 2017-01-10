using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.UI;

namespace Nac.Altseed.Sample.UI
{
	class MessageWindow_Basic : ISample
	{
		public void Run()
		{
			string[] Message = new[]
			{
				"MessageWindow クラスはテキストを\nタイプライター風に表示することのできる\nUIクラスです。",
				"文字列の配列を渡すことで、\n要素ごとにメッセージ送りをしながら\n読むことができます。",
				"MessageWindow.TextObject プロパティ\nから得られるオブジェクトに\nフォントを設定する必要があります。"
			};

			// 同期コンテキストをAltseed用コンテキストに設定
			// Async系メソッドを使うのに必要
			var context = new UpdatableSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(context);

			// Altseedを初期化
			Engine.Initialize("MessageWindow_Basic", 640, 480, new EngineOption());


			// メッセージウィンドウを操作するコントローラーを作成
			var controller = new KeyboardController<int>();
			controller.BindKey(Keys.Z, 0);

			// メッセージウィンドウを作成
			var messageWindow = new MessageWindow();

			// メッセージウィンドウの位置を設定
			messageWindow.Position = new Vector2DF(0, 0);

			// メッセージに用いるフォントを設定
			messageWindow.TextObject.Font = Engine.Graphics.CreateDynamicFont("",
				28,
				new Color(255, 255, 255),
				0,
				new Color(255, 255, 255));

			// メッセージ送りに使うボタンを Z に設定(上で Z=0 という設定をしているので)
			messageWindow.SetReadControl(controller, 0);

			// Altseedにメッセージウィンドウを登録
			Engine.AddObject2D(messageWindow);
			
			// メッセージを表示開始
			messageWindow.TalkMessageAsync(Message);


			// メインループ
			while (Engine.DoEvents())
			{
				Engine.Update();
				// Altseed用コンテキストは毎ループ更新する必要がある
				context.Update();
				Recorder.TakeScreenShot("MessageWindow_Basic", 20);
			}
			
			Engine.Terminate();
		}

		public string Description => "タイプライター風のメッセージを表示するサンプルです。";
		public string Title => "メッセージウィンドウ";
	}
}
