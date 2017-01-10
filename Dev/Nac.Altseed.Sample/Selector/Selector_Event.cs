using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using asd;
using Nac.Altseed.UI.Selector;

namespace Nac.Altseed.Sample.Selector
{
	class Selector_Event : ISample
	{
		public void Run()
		{
			Engine.Initialize("Selector_Basic", 640, 480, new EngineOption());

			// Selectorクラスを作成する。
			var selector = new SimpleSelector<int>();
			// 選択肢オブジェクトの位置同士の間隔を表すベクトルを設定
			selector.ItemSpan = new Vector2DF(0, 40);
			// カーソルのテクスチャを設定
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("Data/Selector/ListCursor.png");
			// 操作方法を設定
			selector.BindKey(Keys.Down, Keys.Up, Keys.Z, Keys.X);
			// Altseedのシステムに追加
			Engine.AddObject2D(selector);

			// 選択肢オブジェクトを作成する
			var font = Engine.Graphics.CreateDynamicFont("", 28, new Color(255, 0, 0), 0, new Color(255, 255, 255));
			for (int i = 0; i < 9; i++)
			{
				var obj = new TextObject2D
				{
					Font = font,
					Text = $"選択肢{i}"
				};
				selector.AddChoice(i, obj);
			}

			// 発生したイベント内容を表示するオブジェクトを作成する
			var display = new TextObject2D();
			display.Font = font;
			display.Position = new Vector2DF(200, 0);
			Engine.AddObject2D(display);

			// 決定時などに鳴らす効果音。
			var moveSound = Engine.Sound.CreateSoundSource("Data/Selector/kachi38.wav", true);
			var decideSound = Engine.Sound.CreateSoundSource("Data/Selector/pi78.wav", true);
			var cancelSound = Engine.Sound.CreateSoundSource("Data/Selector/pi11.wav", true);

			// カーソルを動かしたときに発生する処理を登録する。
			selector.OnMove.Subscribe(index =>
			{
				Engine.Sound.Play(moveSound);
				display.Text = $"{index}番にカーソルを合わせました。";
			});

			// 選択を決定したときに発生する処理を登録する。
			selector.OnDecide.Subscribe(index =>
			{
				Engine.Sound.Play(decideSound);
				display.Text = $"{index}番を選択しました。";
				// 選択UIを破棄する。
				selector.Dispose();
			});

			// 選択をキャンセルしたときに発生する処理を登録する。
			selector.OnCancel.Subscribe(index =>
			{
				Engine.Sound.Play(cancelSound);
				display.Text = $"{index}番を選択中にキャンセルしました。";
				// 選択UIを破棄する。
				selector.Dispose();
			});

			// メインループ
			while (Engine.DoEvents())
			{
				Engine.Update();
				Recorder.TakeScreenShot("Selector_Event", 20);
			}

			Engine.Terminate();
		}

		public string Description => "選択の決定時などに自由に処理を実行するサンプルです。";
		public string Title => "選択UIのイベント";
	}
}
