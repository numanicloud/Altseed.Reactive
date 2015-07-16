using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	public enum AxesDirection
	{
		Negative, Positive
	}

	public class JoystickController<TAbstractKey> : Controller<TAbstractKey>
	{
		abstract class JoystickInput
		{
			public abstract JoystickButtonState GetState(Joystick joystick);

			public virtual void Update(Joystick joystick)
			{
			}
		}

		class ButtonInput : JoystickInput
		{
			private int index;

			public ButtonInput(int index)
			{
				this.index = index;
			}

			public override JoystickButtonState GetState(Joystick joystick)
			{
				return joystick.GetButtonState(index);
			}
		}

		class AxesInput : JoystickInput
		{
			private int axesIndex;
			private int direction;
			private int inputTime;

			public AxesInput(int axesIndex, AxesDirection direction)
			{
				this.axesIndex = axesIndex;
				this.direction = direction == AxesDirection.Positive ? 1 : -1;
				inputTime = 0;
			}

			public override JoystickButtonState GetState(Joystick joystick)
			{
				if (inputTime == -1)
				{
					return JoystickButtonState.Release;
				}
				else if (inputTime == 0)
				{
					return JoystickButtonState.Free;
				}
				else if (inputTime == 1)
				{
					return JoystickButtonState.Push;
				}
				else
				{
					return JoystickButtonState.Hold;
				}
			}

			public override void Update(Joystick joystick)
			{
				if (joystick.GetAxisState(axesIndex) == direction)
				{
					if (inputTime == -1)
					{
						inputTime = 0;
					}
					++inputTime;
				}
				else if (inputTime == -1)
				{
					inputTime = 0;
				}
				else if(inputTime > 0)
				{
					inputTime = -1;
				}
			}
		}

		Joystick joystick;
		Dictionary<TAbstractKey, JoystickInput> binding;

		public override IEnumerable<TAbstractKey> Keys
		{
			get { return binding.Keys; }
		}

		public JoystickController(int index)
		{
			if (!Engine.JoystickContainer.GetIsPresentAt(index))
			{
				throw new InvalidOperationException("指定したインデックスのジョイスティックは接続されていません。");
			}

			joystick = Engine.JoystickContainer.GetJoystickAt(index);
			binding = new Dictionary<TAbstractKey, JoystickInput>();
		}

		public void BindButton(int buttonIndex, TAbstractKey abstractKey)
		{
			binding[abstractKey] = new ButtonInput(buttonIndex);
		}

		public void BindAxes(int axesIndex, AxesDirection direction, TAbstractKey abstractKey)
		{
			binding[abstractKey] = new AxesInput(axesIndex, direction);
		}

		public override InputState? GetState(TAbstractKey key)
		{
			if (binding.ContainsKey(key))
			{
				return ConvertToInputState(binding[key].GetState(joystick));
			}
			else
			{
				return null;
			}
		}

		private InputState ConvertToInputState(JoystickButtonState joystickButtonState)
		{
			switch (joystickButtonState)
			{
			case JoystickButtonState.Push: return InputState.Push;
			case JoystickButtonState.Release: return InputState.Release;
			case JoystickButtonState.Free: return InputState.Free;
			case JoystickButtonState.Hold: return InputState.Hold;
			default: throw new Exception();
			}
		}

		public override void Update()
		{
			foreach (var item in binding)
			{
				item.Value.Update(joystick);
			}
		}
	}
}
