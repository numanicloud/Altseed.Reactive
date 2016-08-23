﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.Sample.Controller
{
	class Controller_Bundle : ISample
	{
		enum MyAction
		{
			Advance, Back, Jump, Attack
		}

		public void Run()
		{
			Engine.Initialize(nameof(Controller_Bundle), 640, 480, new EngineOption());

			var keyboard = new KeyboardController<MyAction>();    // KeyboardControllerクラスを作成
			keyboard.BindKey(Keys.Right, MyAction.Advance);       // 方向キーの右を"Advance"アクションに対応付け
			keyboard.BindKey(Keys.Left, MyAction.Back);           // 方向キーの左を"Back"アクションに対応付け
			keyboard.BindKey(Keys.Z, MyAction.Jump);              // Zキーを		"Jump"アクションに対応付け
			keyboard.BindKey(Keys.X, MyAction.Attack);            // Xキーを		"Attack"アクションに対応付け

			var joystick = new JoystickController<MyAction>(0);				// JoystickControllerクラスを作成
			joystick.BindAxis(0, AxisDirection.Positive, MyAction.Advance);	// 左スティックの右方向への入力を"Advance"アクションに対応付け
			joystick.BindAxis(0, AxisDirection.Negative, MyAction.Back);    // 左スティックの左方向への入力を"Back"アクションに対応付け
			joystick.BindButton(0, MyAction.Jump);                          // ボタン1の入力を"Jump"アクションに対応付け
			joystick.BindButton(1, MyAction.Attack);                        // ボタン2の入力を"Attack"アクションに対応付け

			// 用意したKeyboardControllerとJoystickControllerの入力を合体させた新しいコントローラーを作成
			var bundle = new BundleController<MyAction>(keyboard, joystick);

			//ボタンを押したときに表示するオブジェクトたちを作成
			var font = Engine.Graphics.CreateDynamicFont("", 24, new Color(255, 255, 255, 255), 0, new Color());

			var advanceText = new TextObject2D();
			advanceText.Position = new Vector2DF(0, 0);
			advanceText.Text = "Advance";
			advanceText.Font = font;
			Engine.AddObject2D(advanceText);

			var backText = new TextObject2D();
			backText.Position = new Vector2DF(0, 30);
			backText.Text = "Back";
			backText.Font = font;
			Engine.AddObject2D(backText);

			var jumpText = new TextObject2D();
			jumpText.Position = new Vector2DF(0, 60);
			jumpText.Text = "Jump";
			jumpText.Font = font;
			Engine.AddObject2D(jumpText);

			var attackText = new TextObject2D();
			attackText.Position = new Vector2DF(0, 90);
			attackText.Text = "Attack!";
			attackText.Font = font;
			Engine.AddObject2D(attackText);

			// メインループ
			while(Engine.DoEvents())
			{
				// "Advance"アクションに対応付けられたキーが押されているとき、"Advance"と表示する。以下もほぼ同じ
				// 合体して作ったBundleControllerのGetStateを呼び出せば、登録したコントローラーの入力を統合した結果が得られる
				advanceText.IsDrawn = bundle.GetState(MyAction.Advance) == InputState.Hold;
				backText.IsDrawn = bundle.GetState(MyAction.Back) == InputState.Hold;
				jumpText.IsDrawn = bundle.GetState(MyAction.Jump) == InputState.Hold;
				attackText.IsDrawn = bundle.GetState(MyAction.Attack) == InputState.Hold;

				bundle.Update();	// コントローラーの処理を進める
				Engine.Update();
				Recorder.TakeScreenShot(nameof(Controller_Bundle), 60);
			}

			Engine.Terminate();
		}

		public string Description => "キーボードとジョイスティックの入力を同時に受け取る方法のサンプルです。";
		public string Title => "複数の入力方法の合体";
	}
}
