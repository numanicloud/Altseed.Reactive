using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test.Input
{
	class JoyStickTest
	{
		public void Run()
		{
			asd.Engine.Initialize("JoystickTest", 640, 480, new asd.EngineOption());

			var joystick = asd.Engine.JoystickContainer.GetJoystickAt(0);

			while (asd.Engine.DoEvents())
			{
				asd.Engine.Update();

				Console.WriteLine(joystick.GetButtonState(0));
			}

			asd.Engine.Terminate();
		}
	}
}
