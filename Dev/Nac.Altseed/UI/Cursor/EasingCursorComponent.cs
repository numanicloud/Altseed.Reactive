using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
	public class EasingCursorComponent : CursorComponent
	{
		private IDisposable animationSubscription;
		private Subject<Unit> onUpdate;

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
		public EasingCursorComponent()
		{
			onUpdate = new Subject<Unit>();
			Easing = new CubicEasing(CubicEasing.Speed.Rapidly2, CubicEasing.Speed.Slowly3);
			EasingDurationFrame = 10;
		}

		protected override void OnUpdate()
		{
			onUpdate.OnNext(Unit.Default);
		}

		/// <summary>
		/// オーバーライドして、指定したオブジェクトへ移動するアニメーションを実装できます。
		/// </summary>
		protected override void AnimateMove()
		{
			animationSubscription?.Dispose();
			var initial = Owner.Position;
			var goal = new Vector2DF(0, 0);
			animationSubscription = onUpdate.Select((x, i) =>
					Easing.GetGeneralValue(i, EasingDurationFrame, 0, 1))
				.Select(x => initial * (1 - x) + goal * x)
				.Subscribe(x => Owner.Position = x);
		}
	}
}
