using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Easings;
using Nac.Altseed.Easings.Library;

namespace Nac.Altseed.UI.Cursor
{
	/// <summary>
	/// 他のオブジェクトに滑らかに追従するカーソルを描画するオブジェクト。
	/// </summary>
	public class EasingCursor : Cursor
	{
		private IDisposable animationSubscription;

		/// <summary>
		/// アニメーションに用いるイージングを表すインスタンスを取得または設定します。
		/// </summary>
		public IEasing Easing { get; set; }
		/// <summary>
		/// アニメーションに要する時間(フレーム)を取得または設定します。
		/// </summary>
		public int EasingDurationFrame { get; set; }

		/// <summary>
		/// 初期値を使用して、EasingCursor の新しいインスタンスを生成します。
		/// </summary>
		public EasingCursor()
		{
			Easing = new CubicEasing(CubicEasing.Speed.Rapidly2, CubicEasing.Speed.Slowly3);
			EasingDurationFrame = 10;
		}

		/// <summary>
		/// オーバーライドして、指定したオブジェクトへ移動するアニメーションを実装できます。
		/// </summary>
		/// <param name="obj">追従する先となるオブジェクト。</param>
		protected override void AnimateMove(Object2D obj)
		{
			animationSubscription?.Dispose();
			var initial = Position;
			animationSubscription = OnUpdateEvent.Select((x, i) =>
					Easing.GetGeneralValue(i, EasingDurationFrame, 0, 1))
				.Select(x => initial * (1 - x) + obj.Position * x)
				.Subscribe(x => Position = x);
		}
	}
}
