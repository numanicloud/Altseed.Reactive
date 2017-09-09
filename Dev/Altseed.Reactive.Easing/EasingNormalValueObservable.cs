using System;
using System.Linq;
using System.Reactive.Linq;

namespace Altseed.Reactive.Easings
{
	/// <summary>
	/// イージングの0~1の値が発行される IObservable の実装を提供します。
	/// </summary>
	public class EasingNormalValueObservable : IObservable<float>
	{
		private IEasing easing;
		private IObservable<float> source;

		/// <summary>
		/// 時間を表すプロバイダーとアニメーション内容を表すイージングを指定して、このプロバイダーを初期化します。
		/// </summary>
		/// <param name="source">[0,1]間の現在の時間が通知されるプロバイダー。</param>
		/// <param name="easing">アニメーション内容を表すイージング関数。</param>
		public EasingNormalValueObservable(IObservable<float> source, IEasing easing)
		{
			this.source = source;
			this.easing = easing;
		}

		/// <summary>
		/// オブザーバーが通知を受け取ることをプロバイダーに通知します。
		/// </summary>
		/// <param name="observer">通知を受け取るオブジェクト。</param>
		/// <returns>プロバイダーが通知の送信を完了する前に、オブザーバーが通知の受信を停止できるインターフェイスへの参照。</returns>
		public IDisposable Subscribe(IObserver<float> observer)
		{
			return source.TakeWhile(t => t <= 1)
				.Select(easing.GetNormalValue)
				.Subscribe(observer);
		}

		/// <summary>
		/// [0,1]の時間内に[0,1]の範囲でアニメーションする値を、任意の時間内に任意の範囲でアニメーションする値に射影します。
		/// </summary>
		/// <param name="fullDuration">アニメーション全体が要する時間。</param>
		/// <param name="start">アニメーション始点の値。</param>
		/// <param name="goal">アニメーション終点の値。</param>
		/// <returns></returns>
		public IObservable<float> Generalize(float fullDuration, float start, float goal)
		{
			return source.TakeWhile(t => t <= fullDuration)
				.Select(t => easing.GetGeneralValue(t, fullDuration, start, goal));
		}
	}
}
