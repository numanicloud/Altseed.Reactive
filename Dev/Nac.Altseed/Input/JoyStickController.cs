using System;
using System.Collections.Generic;
using asd;

namespace Nac.Altseed.Input
{
	/// <summary>
	/// ゲームパッドの軸を倒す方向を表す列挙体。
	/// </summary>
	public enum AxisDirection
	{
		/// <summary>
		/// 正の方向に倒すことを表します。
		/// </summary>
		Negative,
		/// <summary>
		/// 負の方向に倒すことを表します。
		/// </summary>
		Positive
	}

	/// <summary>
	/// ジョイスティックからの入力を操作と対応付けるコントローラ クラス。
	/// </summary>
	/// <typeparam name="TAbstractKey">操作を表す型。</typeparam>
	/// <remarks>スティック入力の加減を取得する用途には、Altseedのジョイスティック機能を直接お使いください。</remarks>
	public class JoystickController<TAbstractKey> : Controller<TAbstractKey>
	{
		abstract class JoystickInput
		{
			public abstract ButtonState GetState(Joystick joystick);

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

			public override ButtonState GetState(Joystick joystick)
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

			public override ButtonState GetState(Joystick joystick)
			{
				if (currentState)
				{
					return previousState ? ButtonState.Hold : ButtonState.Push;
				}
				else
				{
					return previousState ? ButtonState.Release : ButtonState.Free;
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

		public bool IsValid { get; private set; }

		/// <summary>
		/// なんらかの入力に対応付けられている操作のコレクションを取得します。
		/// </summary>
		public override IEnumerable<TAbstractKey> Keys
		{
			get { return binding.Keys; }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="index"></param>
		public JoystickController(int index)
		{
			IsValid = true;
			if (!Engine.JoystickContainer.GetIsPresentAt(index))
			{
				IsValid = false;
			}

			joystick = Engine.JoystickContainer.GetJoystickAt(index);
			binding = new Dictionary<TAbstractKey, JoystickInput>();
		}

		/// <summary>
		/// ジョイスティックのボタンに操作を対応付けます。
		/// </summary>
		/// <param name="buttonIndex">操作に対応付けるボタンの番号。</param>
		/// <param name="abstractKey">ボタンに対応付ける操作。</param>
		public void BindButton(int buttonIndex, TAbstractKey abstractKey)
		{
			binding[abstractKey] = new ButtonInput(buttonIndex);
		}

		/// <summary>
		/// ジョイスティックのスティックに操作を対応付けます。
		/// </summary>
		/// <param name="axisIndex">操作に対応付ける軸の番号。通常 0 がX軸を、1 がY軸を表します。</param>
		/// <param name="direction">スティックをどちらに倒した場合の入力を対応付けるか。</param>
		/// <param name="abstractKey">スティックの入力に対応付ける操作。</param>
		public void BindAxis(int axisIndex, AxisDirection direction, TAbstractKey abstractKey)
		{
			binding[abstractKey] = new AxisInput(axisIndex, direction);
		}

		/// <summary>
		/// ジョイスティックのスティックの上下左右の入力に操作を対応付けます。
		/// </summary>
		/// <param name="left">左入力に対応付ける操作。</param>
		/// <param name="right">右入力に対応付ける操作。</param>
		/// <param name="up">上入力に対応付ける操作。</param>
		/// <param name="down">下入力に対応付ける操作。</param>
		public void BindDirection(TAbstractKey left, TAbstractKey right, TAbstractKey up, TAbstractKey down)
		{
			BindAxis(0, AxisDirection.Negative, left);
			BindAxis(0, AxisDirection.Positive, right);
			BindAxis(1, AxisDirection.Negative, up);
			BindAxis(1, AxisDirection.Positive, down);
		}

		/// <summary>
		/// 指定した操作に対応する入力の状態を取得します。
		/// </summary>
		/// <param name="key">入力の状態を取得する操作。</param>
		/// <returns></returns>
		public override InputState? GetState(TAbstractKey key)
		{
			if (!IsValid)
			{
				return null;
			}
			if (binding.ContainsKey(key))
			{
				var result = ConvertToInputState(binding[key].GetState(joystick));
				return result;
			}
			else
			{
				return null;
			}
		}

		private InputState ConvertToInputState(ButtonState joystickButtonState)
		{
			switch (joystickButtonState)
			{
			case ButtonState.Push: return InputState.Push;
			case ButtonState.Release: return InputState.Release;
			case ButtonState.Free: return InputState.Free;
			case ButtonState.Hold: return InputState.Hold;
			default: throw new Exception();
			}
		}

		/// <summary>
		/// コントローラの状態を更新します。
		/// </summary>
		public override void Update()
		{
			if (!IsValid)
			{
				return;
			}
			foreach (var item in binding)
			{
				item.Value.Update(joystick);
			}
		}
	}
}
