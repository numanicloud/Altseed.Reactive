using asd;
using Nac.Altseed.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test.Input
{
	class BundleControllerTest : AltseedTest
	{
		private TextObject2D textObject;
		private BundleController<int> controller;

		protected override void OnStart()
		{
			textObject = new TextObject2D()
			{
				Font = Engine.Graphics.CreateDynamicFont("", 24, new Color(255, 255, 255), 0, new Color()),
				Position = new Vector2DF(10, 10),
			};
			Engine.AddObject2D(textObject);

			var keyboard = new KeyboardController<int>();
			keyboard.BindKey(Keys.Z, 0);
			var joystick = new JoystickController<int>(0);
			joystick.BindButton(0, 0);

			controller = new BundleController<int>(keyboard, joystick);
		}

		protected override void OnUpdate()
		{
			controller.Update();

			string message = "";
			switch (controller.GetState(0))
			{
			case InputState.Push:
				message = "Push";
				break;
			case InputState.Hold:
				message = "Hold";
				break;
			case InputState.Release:
				message = "Release";
				break;
			case InputState.Free:
				message = "Free";
				break;
			default:
				break;
			}

			textObject.Text = message;
		}
	}
}
