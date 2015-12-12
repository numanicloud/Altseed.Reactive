using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Input
{
	public static class ControllerHelper
	{
		public static IObservable<Unit> ObserveKeyState<TAbstractKey>(this Controller<TAbstractKey> controller, TAbstractKey key, InputState state)
		{
			return UpdateManager.Instance.FrameUpdate
				.Where(x => controller.GetState(key) == state)
				.Select(x => Unit.Default);
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
