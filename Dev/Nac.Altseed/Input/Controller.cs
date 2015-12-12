using System.Collections.Generic;

namespace Nac.Altseed.Input
{
	/// <summary>
	/// コントローラのボタンの状態を表す列挙体。
	/// </summary>
	public enum InputState
	{
		/// <summary>
		/// ボタンが押された瞬間を表します。
		/// </summary>
		Push,
		/// <summary>
		/// ボタンが離された瞬間を表します。
		/// </summary>
		Release,
		/// <summary>
		/// ボタンが押されたままになっている状態を表します。
		/// </summary>
		Hold,
		/// <summary>
		/// ボタンが離されている状態を表します。
		/// </summary>
		Free
	}

	/// <summary>
	/// 実際の入力を操作と対応付けるクラス。
	/// </summary>
	/// <typeparam name="TControl">操作を表す型。</typeparam>
	public abstract class Controller<TControl>
	{
		/// <summary>
		/// なんらかの入力に対応付けられている操作のコレクションを取得します。
		/// </summary>
		public abstract	IEnumerable<TControl> Keys { get; }

		/// <summary>
		/// 指定した操作に対応する入力の状態を取得します。
		/// </summary>
		/// <param name="key">入力の状態を取得する操作。</param>
		/// <returns></returns>
		public abstract InputState? GetState(TControl key);

		/// <summary>
		/// コントローラの状態を更新します。
		/// </summary>
		public virtual void Update()
		{
		}
	}
}
