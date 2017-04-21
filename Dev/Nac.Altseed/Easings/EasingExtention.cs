using Nac.Altseed.Easings.Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Easings
{
	/// <summary>
	/// イージング機能に対する拡張メソッドを公開します。
	/// </summary>
	public static class EasingExtention
	{
		/// <summary>
		/// 指定した条件におけるイージングの値を取得します。
		/// </summary>
		/// <param name="easing">イージング クラス。</param>
		/// <param name="time">イージング中における現在の時間。</param>
		/// <param name="fullDuration">イージング開始から終了までの全体の時間。</param>
		/// <param name="start">イージングの始点の値。</param>
		/// <param name="goal">イージングの終点の値。</param>
		/// <returns></returns>
		public static float GetGeneralValue(this IEasing easing, float time, float fullDuration, float start, float goal)
		{
			if (time > fullDuration)
			{
				return goal;
			}
			else if(time < 0)
			{
				return start;
			}
			return easing.GetNormalValue(time / fullDuration) * (goal - start) + start; 
		}

		/// <summary>
		/// 指定したプロバイダーを、三次曲線によってアニメーションする値が通知されるプロバイダーに射影します。
		/// </summary>
		/// <param name="timeSource">区間[0,1]の現在の時間が通知されるプロバイダー。</param>
		/// <param name="startSpeed">始点の速度。</param>
		/// <param name="endSpeed">終点の速度。</param>
		/// <returns>三次曲線によってアニメーションする正規化された値が通知されるプロバイダー。</returns>
		public static EasingNormalValueObservable CubicEasingsNormalValue(
			this IObservable<float> timeSource,
			CubicEasing.Speed startSpeed,
			CubicEasing.Speed endSpeed)
		{
			var cubic = new CubicEasing(startSpeed, endSpeed);
			return new EasingNormalValueObservable(timeSource, cubic);
		}

		/// <summary>
		/// 指定したプロバイダーを、三次曲線によってアニメーションする値が通知されるプロバイダーに射影します。
		/// </summary>
		/// <param name="timeSource">区間[0,1]の現在の時間が通知されるプロバイダー。</param>
		/// <returns></returns>
		public static EasingNormalValueObservable CubicEasingsNormalValue(this IObservable<float> timeSource)
		{
			var cubic = new CubicEasing();
			return new EasingNormalValueObservable(timeSource, cubic);
		}

		/// <summary>
		/// 指定したプロバイダーを、線形にアニメーションする値が通知されるプロバイダーに射影します。
		/// </summary>
		/// <param name="timeSource">区間[0,1]の現在の時間が通知されるプロバイダー。</param>
		/// <returns>線形にアニメーションする正規化された値が通知されるプロバイダー。</returns>
		public static EasingNormalValueObservable LinearEasingNormalValue(this IObservable<float> timeSource)
		{
			var linear = new LinearEasing();
			return new EasingNormalValueObservable(timeSource, linear);
		}
	}
}
