using System.Collections.Generic;
using asd;

namespace Nac.Altseed.Input
{
	/// <summary>
	/// キーボードからの入力を操作と対応付けるコントローラ クラス。
	/// </summary>
	/// <typeparam name="TControl">操作を表す型。</typeparam>
	public class KeyboardController<TControl> : Controller<TControl>
	{
		private Dictionary<TControl, Keys> binding;

		/// <summary>
		/// なんらかの入力に対応付けられている操作のコレクションを取得します。
		/// </summary>
		public override IEnumerable<TControl> Keys
		{
			get { return binding.Keys; }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public KeyboardController()
		{
			binding = new Dictionary<TControl, Keys>();
		}

		/// <summary>
		/// キーボードのキーに操作を対応付けます。
		/// </summary>
		/// <param name="key">操作に対応付けるキー。</param>
		/// <param name="abstractKey">キーに対応付ける操作。</param>
		public void BindKey(Keys key, TControl abstractKey)
		{
			binding[abstractKey] = key;
		}

		/// <summary>
		/// 十字キーの入力に操作を対応付けます。
		/// </summary>
		/// <param name="left">左キーに対応付ける操作。</param>
		/// <param name="right">右キーに対応付ける操作。</param>
		/// <param name="up">上キーに対応付ける操作。</param>
		/// <param name="down">下キーに対応付ける操作。</param>
		public void BindDirection(TControl left, TControl right, TControl up, TControl down)
		{
			binding[left] = asd.Keys.Left;
			binding[right] = asd.Keys.Right;
			binding[up] = asd.Keys.Up;
			binding[down] = asd.Keys.Down;
		}

		/// <summary>
		/// 指定した操作に対応する入力の状態を取得します。
		/// </summary>
		/// <param name="key">入力の状態を取得する操作。</param>
		/// <returns></returns>
		public override InputState? GetState(TControl key)
		{
			if (binding.ContainsKey(key))
			{
				return (InputState)Engine.Keyboard.GetKeyState(binding[key]);
			}
			else
			{
				return null;
			}
		}
	}
}
