using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Input
{
	class KeyboardInputTest
	{
		// 操作を表す列挙体
		enum Control
		{
			Left, Right, Jump, Attack
		}

		// 文字列オブジェクトを作成するヘルパー関数
		private static asd.TextObject2D CreateLabel(string name, float y, asd.Font font)
		{
			return new asd.TextObject2D
			{
				Text = name,
				Font = font,
				Position = new asd.Vector2DF(10, y),
			};
		}

		public void Run()
		{
			asd.Engine.Initialize("KeyboardInputTest", 640, 480, new asd.EngineOption());

			var font = asd.Engine.Graphics.CreateDynamicFont("", 32, new asd.Color(255, 255, 255), 0, new asd.Color(0, 0, 0));

			// 文字列オブジェクトを生成しておく
			var left = CreateLabel("Left", 10, font);
			var right = CreateLabel("Right", 40, font);
			var jump = CreateLabel("Jump", 70, font);
			var attack = CreateLabel("Attack", 100, font);
			asd.Engine.AddObject2D(left);
			asd.Engine.AddObject2D(right);
			asd.Engine.AddObject2D(jump);
			asd.Engine.AddObject2D(attack);

			// コントローラーを初期化
			var input = new Altseed.Reactive.Input.KeyboardController<Control>();

			// 操作とキーを結びつける
			input.BindKey(asd.Keys.Left, Control.Left);		// 左キーを左移動に結びつける
			input.BindKey(asd.Keys.Right, Control.Right);	// 右キーを右移動に結びつける
			input.BindKey(asd.Keys.Z, Control.Jump);		// Z キーをジャンプに結びつける
			input.BindKey(asd.Keys.X, Control.Attack);      // X キーを攻撃に結びつける

			asd.Engine.CaptureScreenAsGifAnimation("Output/KeyboardInputTest.gif", 90, 0.5f, 0.5f);

			while (asd.Engine.DoEvents())
			{
				// 押しているキーに対応づいた文字列だけを表示する
				left.IsDrawn = input.GetState(Control.Left) == Reactive.Input.InputState.Hold;
				right.IsDrawn = input.GetState(Control.Right) == Reactive.Input.InputState.Hold;
				jump.IsDrawn = input.GetState(Control.Jump) == Reactive.Input.InputState.Hold;
				attack.IsDrawn = input.GetState(Control.Attack) == Reactive.Input.InputState.Hold;

				asd.Engine.Update();
			}

			asd.Engine.Terminate();
		}
	}
}
