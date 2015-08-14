using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	public enum AxisDirection
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

		class AxisInput : JoystickInput
		{
			private int axisIndex;
			private int direction;
			private bool previousState;
			private bool currentState;

			public AxisInput(int axisIndex, AxisDirection direction)
			{
				this.axisIndex = axisIndex;
				this.direction = direction == AxisDirection.Positive ? 1 : -1;
				previousState = false;
				currentState = false;
			}

			public override JoystickButtonState GetState(Joystick joystick)
			{
				if (currentState)
				{
					return previousState ? JoystickButtonState.Hold : JoystickButtonState.Push;
				}
				else
				{
					return previousState ? JoystickButtonState.Release : JoystickButtonState.Free;
				}
			}

			public override void Update(Joystick joystick)
			{
				previousState = currentState;
				currentState = joystick.GetAxisState(axisIndex) == direction;
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

		public void BindAxis(int axisIndex, AxisDirection direction, TAbstractKey abstractKey)
		{
			binding[abstractKey] = new AxisInput(axisIndex, direction);
		}

		public void BindDirection(TAbstractKey left, TAbstractKey right, TAbstractKey up, TAbstractKey down)
		{
			BindAxis(0, AxisDirection.Negative, left);
			BindAxis(0, AxisDirection.Positive, right);
			BindAxis(1, AxisDirection.Negative, up);
			BindAxis(1, AxisDirection.Positive, down);
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
