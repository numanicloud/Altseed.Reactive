using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.Sample.Controller
{
	class Controller_Stepping : ISample
	{
		enum MyAction
		{
			Plus, Minus
		}

		public void Run()
		{
			Engine.Initialize(nameof(Controller_Bundle), 640, 480, new EngineOption());

			var keyboard = new KeyboardController<MyAction>();	// KeyboardControllerクラスを作成
			keyboard.BindKey(Keys.Up, MyAction.Plus);			// 方向キーの上を"Plus"アクションに対応付け
			keyboard.BindKey(Keys.Down, MyAction.Minus);        // 方向キーの下を"Minus"アクションに対応付け

			// SteppingControllerクラスを作成
			var stepping = new SteppingController<MyAction>(keyboard);
			// "Plus"アクションを30Fの間押しっぱなしにしたら、2フレーム間隔で連続入力するよう設定
			stepping.EnableSteppingHold(MyAction.Plus, 30, 2);
			// "Minus"アクションを30Fの間押しっぱなしにしたら、2フレーム間隔で連続入力するよう設定
			stepping.EnableSteppingHold(MyAction.Minus, 30, 2);
			
			// アクションによって増減する値を表示するオブジェクトを作成する。
			var font = Engine.Graphics.CreateDynamicFont("", 24, new Color(255, 255, 255, 255), 0, new Color());
			int count = 10;
			var numText = new TextObject2D();
			numText.Position = new Vector2DF(0, 0);
			numText.Text = $"↑\n{count}\n↓";
			numText.Font = font;
			Engine.AddObject2D(numText);

			// メインループ
			while(Engine.DoEvents())
			{
				if (stepping.GetState(MyAction.Plus) == InputState.Push)
				{
					count++;
				}
				else if (stepping.GetState(MyAction.Minus) == InputState.Push)
				{
					count--;
				}
				numText.Text = $"↑\n{count}\n↓";

				stepping.Update();	// コントローラーの処理を進める
				Engine.Update();
				Recorder.TakeScreenShot(nameof(Controller_Bundle), 60);
			}

			Engine.Terminate();
		}

		public string Description => "キーを押しっぱなしにしたときに連続でアクションを起こすことができる\nコントローラーのサンプルです。";
		public string Title => "押しっぱなしを連続入力に";
	}
}
