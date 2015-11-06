using asd;
using Nac.Altseed.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test
{
	class Program
	{
		enum Control
		{
			Left, Right, Jump, ScreenShot, Disabled
		}

		static void Main(string[] args)
		{
			Engine.Initialize("Test", 640, 480, new asd.EngineOption());

			var joystick = new JoystickController<Control>(0);
			joystick.BindAxis(0, AxisDirection.Negative, Control.Left);
			joystick.BindAxis(0, AxisDirection.Positive, Control.Right);
			joystick.BindButton(0, Control.Jump);

			var keyboard = new KeyboardController<Control>();
			keyboard.BindKey(Keys.Left, Control.Left);
			keyboard.BindKey(Keys.Right, Control.Right);
			keyboard.BindKey(Keys.Z, Control.Jump);
			keyboard.BindKey(Keys.P, Control.ScreenShot);

			var bundle = new BundleController<Control>(joystick, keyboard);

			var step = new SteppingController<Control>(bundle);
			step.EnableSteppingHold(Control.Left, 30, 10);
			step.EnableSteppingHold(Control.Right, 30, 10);
			//step.EnableSteppingHold(Control.Jump, 60, 30);
			step.EnableSteppingHold(Control.Disabled, 60, 30);

			var choice = new Choice<Control>(4, step);
			choice.BindKey(Control.Left, ChoiceControll.Previous);
			choice.BindKey(Control.Right, ChoiceControll.Next);
			choice.BindKey(Control.Jump, ChoiceControll.Decide);
			choice.OnMove += (prev, index) => Console.WriteLine("{0} -> {1}", prev, index);
			choice.OnDecide += index => Console.WriteLine("Decide:{0}", index);
			choice.Loop = true;
			choice.AddSkippedIndex(1);

			for (int i = 0; i < 180; i++)
			{
				Engine.DoEvents();
				Engine.Update();
				step.Update();
			}

			Console.WriteLine("Start");

			while (Engine.DoEvents())
			{
				/*
				if (step.GetState(Control.Left) == InputState.Push)
				{
					Console.WriteLine("Move Push");
				}
				if (step.GetState(Control.Left) == InputState.Release)
				{
					Console.WriteLine("Move Release");
				}
				if (step.GetState(Control.Jump) == InputState.Push)
				{
					Console.WriteLine("Jump");
				}
				//*/
				if (step.GetState(Control.ScreenShot) == InputState.Push)
				{
					Console.WriteLine("ScreenShot");
				}
				//*/

				//joystick.Update();
				step.Update();
				choice.Update();
				Engine.Update();
			}

			Engine.Terminate();
		}
	}
}
