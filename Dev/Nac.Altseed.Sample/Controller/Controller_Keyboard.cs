using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.UI.Selector;

namespace Nac.Altseed.Sample.Controller
{
	public class Controller_Keyboard : ISample
	{
		enum MyAction
		{
			Advance, Back, Jump, Attack
		}

		// キーが押されていれば黄色を、そうでなければ白を返す関数。
		public Color KeyStateToColor(InputState? state)
		{
			return state == InputState.Hold ? new Color(255, 255, 0, 255) : new Color(255, 255, 255, 255);
		}

		public void Run()
		{
			/*
			 * KeyboardControllerなどの型引数にユーザー独自の列挙型などを指定することで
			 * 物理的な「キー」と実際の「アクション」を対応付けることによって、
			 * キーとアクションの対応付けを実行中に様々に切り替えることが容易になります。
			 * たとえばキーコンフィグ機能の作成に活用することができます。
			 */

			Engine.Initialize(nameof(Controller_Keyboard), 640, 480, new EngineOption());
			
			var controller = new KeyboardController<MyAction>();	// KeyboardControllerクラスを作成
			controller.BindKey(Keys.Right, MyAction.Advance);		// 方向キーの右を"Advance"アクションに対応付け
			controller.BindKey(Keys.Left, MyAction.Back);			// 方向キーの左を"Back"アクションに対応付け
			controller.BindKey(Keys.Z, MyAction.Jump);				// Zキーを		"Jump"アクションに対応付け
			controller.BindKey(Keys.X, MyAction.Attack);			// Xキーを		"Attack"アクションに対応付け

			//ボタンを押したときに表示するオブジェクトたちを作成
			var font = Engine.Graphics.CreateDynamicFont("", 24, new Color(255, 255, 255, 255), 0, new Color());

			// あとで右キーを押したときに表示するようにするためのオブジェクトです。
			var advanceText = new TextObject2D();
			advanceText.Position = new Vector2DF(0, 0);
			advanceText.Text = "Advance";
			advanceText.Font = font;
			Engine.AddObject2D(advanceText);

			// あとで左キーを押したときに表示するようにするためのオブジェクトです。
			var backText = new TextObject2D();
			backText.Position = new Vector2DF(0, 30);
			backText.Text = "Back";
			backText.Font = font;
			Engine.AddObject2D(backText);

			// あとでZキーを押したときに表示するようにするためのオブジェクトです。
			var jumpText = new TextObject2D();
			jumpText.Position = new Vector2DF(0, 60);
			jumpText.Text = "Jump";
			jumpText.Font = font;
			Engine.AddObject2D(jumpText);


			// あとでXキーを押したときに表示するようにするためのオブジェクトです。
			var attackText = new TextObject2D();
			attackText.Position = new Vector2DF(0, 90);
			attackText.Text = "Attack!";
			attackText.Font = font;
			Engine.AddObject2D(attackText);

			// メインループ
			while(Engine.DoEvents())
			{
				// "Advance"アクションに対応付けられたキー(=右キー)が押されているとき、"Advance"の文字列を黄色くする。
				advanceText.Color = KeyStateToColor(controller.GetState(MyAction.Advance));
				// "Back"アクションに対応付けられたキー(=左キー)が押されているとき、"Advance"の文字列を黄色くする。
				backText.Color = KeyStateToColor(controller.GetState(MyAction.Back));
				// "Jump"アクションに対応付けられたキー(=Zキー)が押されているとき、"Advance"の文字列を黄色くする。
				jumpText.Color = KeyStateToColor(controller.GetState(MyAction.Jump));
				// "Attack"アクションに対応付けられたキー(=Xキー)が押されているとき、"Advance"の文字列を黄色くする。
				attackText.Color = KeyStateToColor(controller.GetState(MyAction.Attack));

				Engine.Update();
				Recorder.TakeScreenShot(nameof(Controller_Keyboard), 60);
			}

			Engine.Terminate();
		}

		public string Description => "Controller系クラスの基本的な使い方のサンプルです。";
		public string Title => "Controller系クラス";
	}
}
