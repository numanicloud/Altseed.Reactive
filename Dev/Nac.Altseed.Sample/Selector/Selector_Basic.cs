﻿using asd;						// Engineクラスなどをフルネームで書かずに済ますように
using Nac.Altseed.UI.Selector;  // SimpleSelectorクラスなどをフルネームで書かずに済ますように

namespace Nac.Altseed.Sample.Selector
{
	class Selector_Basic : ISample
	{
		public void Run()
		{
			Engine.Initialize("Selector_Basic", 640, 480, new EngineOption());

			var selector = new SimpleSelector<int>();				// Selectorクラスを作成する。
			selector.ItemSpan = new Vector2DF(0, 40);				// 選択肢オブジェクトの位置同士の間のベクトル
			selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("Data/Selector/ListCursor.png");	// カーソルのテクスチャを設定
			selector.BindKey(Keys.Down, Keys.Up, Keys.Z, Keys.X);	// 操作方法を設定
			Engine.AddObject2D(selector);							// Altseedのシステムに追加

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
			
			// メインループ
			while(Engine.DoEvents())
			{
				Engine.Update();
				Recorder.TakeScreenShot("Selector_Basic", 20);
			}
			
			Engine.Terminate();
		}

		public string Description => "セレクターの使い方のサンプルです。";
		public string Title => "セレクターの基本";
	}
}
