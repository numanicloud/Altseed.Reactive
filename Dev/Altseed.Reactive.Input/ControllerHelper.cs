using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using asd;

namespace Altseed.Reactive.Input
{
	public static class ControllerHelper
	{
		/// <summary>
		/// ストリームを、指定したコントローラーの状態に基いてフィルターします。
		/// </summary>
		/// <typeparam name="T">元となるストリームの型。</typeparam>
		/// <typeparam name="TAbstractKey">操作の型。</typeparam>
		/// <param name="source">元となるストリーム。</param>
		/// <param name="controller">チェックするコントローラー。</param>
		/// <param name="key">チェックする操作。</param>
		/// <param name="state">フィルター条件となる操作の状態。</param>
		/// <returns></returns>
		public static IObservable<T> ObserveKeyState<T, TAbstractKey>(
			this IObservable<T> source,
			Controller<TAbstractKey> controller,
			TAbstractKey key,
			InputState state)
		{
			return source.Where(x => controller.GetState(key) == state);
		}

		/// <summary>
		/// コントローラの方向キーが押されている向きをベクトルとして取得します。返すベクトルは正規化されません。
		/// </summary>
		/// <typeparam name="TControl"></typeparam>
		/// <param name="controller">コントローラ。</param>
		/// <param name="left">左入力を表す操作。</param>
		/// <param name="right">右入力を表す操作。</param>
		/// <param name="up">上入力を表す操作。</param>
		/// <param name="down">下入力を表す操作。</param>
		/// <returns></returns>
		public static Vector2DF GetInputDirection<TControl>(this Controller<TControl> controller, TControl left, TControl right, TControl up, TControl down)
		{
			var direction = new Vector2DF(0, 0);
			if(controller.GetState(left) == InputState.Hold)
			{
				direction.X -= 1;
			}
			if(controller.GetState(right) == InputState.Hold)
			{
				direction.X += 1;
			}
			if(controller.GetState(up) == InputState.Hold)
			{
				direction.Y -= 1;
			}
			if(controller.GetState(down) == InputState.Hold)
			{
				direction.Y += 1;
			}
			return direction;
		}
	}
}
