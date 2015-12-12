using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Linq
{
	public static class ObservableHelper
	{
		/// <summary>
		/// 呼びだされてからの経過時間が流れ続けるシーケンスを生成します。
		/// </summary>
		/// <returns></returns>
		public static IObservable<float> CountTime()
		{
			float time = 0;
			return UpdateManager.Instance.FrameUpdate
				.Do(f => time += f)
				.Select(x => time);
		}

		/// <summary>
		/// 呼びだされてからの経過フレーム数が流れ続けるシーケンスを生成します。
		/// </summary>
		/// <returns></returns>
		public static IObservable<int> CountFrame()
		{
			int frame = 0;
			return UpdateManager.Instance.FrameUpdate
				.Do(f => frame++)
				.Select(x => frame);
		}

		/// <summary>
		/// 流れてきた値を累計して後続に流します。
		/// </summary>
		/// <param name="source">累計される値のシーケンス。</param>
		/// <returns></returns>
		public static IObservable<float> Total(this IObservable<float> source)
		{
			float total = 0;
			return source.Do(f => total += f).Select(x => total);
		}

		/// <summary>
		/// シーケンスをコルーチンの呼び出しにマッピングします。イベントの発行がコルーチンを１回進めます。
		/// </summary>
		/// <typeparam name="T">シーケンスの型。</typeparam>
		/// <typeparam name="U">コルーチンの返す型。</typeparam>
		/// <param name="source">元となるシーケンス。</param>
		/// <param name="coroutine">シーケンスにマップするコルーチン。</param>
		/// <returns>コルーチンにマッピングされたシーケンス。</returns>
		public static IObservable<U> SelectCorourine<T, U>(this IObservable<T> source, IEnumerator<U> coroutine)
		{
			return source.TakeWhile(x => coroutine.MoveNext())
				.Select(x => coroutine.Current);
		}

		/// <summary>
		/// コルーチンを毎フレーム呼び出すシーケンスを生成します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="coroutine">毎フレーム進めるコルーチン。</param>
		/// <returns></returns>
		public static IObservable<T> FromCoroutine<T>(IEnumerator<T> coroutine)
		{
			return UpdateManager.Instance.FrameUpdate
				.TakeWhile(x => coroutine.MoveNext())
				.Select(x => coroutine.Current);
		}

		/// <summary>
		/// シーケンスを時間単位として、なめらかに変化するベクトルの値が流れるシーケンスに変換します。
		/// </summary>
		/// <param name="source">前フレームの値が通るシーケンス。値を通すことで時間が進みます。</param>
		/// <param name="goal">最終フレームの値。</param>
		/// <param name="start">始点の速度。</param>
		/// <param name="end">終点の速度。</param>
		/// <param name="count">終点までのフレーム数。</param>
		/// <returns></returns>
		public static IObservable<float> EasingValue(this IObservable<float> source, float goal, EasingStart start, EasingEnd end, int count)
		{
			var f = Easing.GetEasingFunction(start, end);
			return source.Select((x, i) => Easing.GetNextValue(x, goal, i, count, f))
				.Take(count + 1);
		}

		/// <summary>
		/// シーケンスを時間単位として、なめらかに変化するベクトルの値が流れるシーケンスに変換します。
		/// </summary>
		/// <param name="source">前フレームの値が通るシーケンス。値を通すことで時間が進みます。</param>
		/// <param name="goal">最終フレームの値。</param>
		/// <param name="start">始点の速度。</param>
		/// <param name="end">終点の速度。</param>
		/// <param name="count">終点までのフレーム数。</param>
		/// <returns></returns>
		public static IObservable<Vector2DF> EasingVector2DF(this IObservable<Vector2DF> source, Vector2DF goal, EasingStart start, EasingEnd end, int count)
		{
			var f = Easing.GetEasingFunction(start, end);
			return source.Select((v, i) => Easing.GetNextValue(v, goal, i, count, f))
				.Take(count + 1);
		}

		/// <summary>
		/// シーケンスを時間単位として、時間によって振動するベクトルの値が流れるシーケンスに変換します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">値が通ることで時間を進めるシーケンス。</param>
		/// <param name="center">振動の中心点。</param>
		/// <param name="amplitude">軸ごとの振幅。</param>
		/// <param name="frequency">軸ごとの周波数。</param>
		/// <param name="count">振動が終了するまでの時間。</param>
		/// <returns></returns>
		public static IObservable<Vector2DF> ShortWiggle<T>(this IObservable<T> source, Vector2DF center, Vector2DF amplitude, Vector2DF frequency, int count)
		{
			var fps = Engine.TargetFPS;
			return source.Select((x, i) => (float)i)
				.TakeWhile(t => t < count)
				.Select(t => new
				{
					Phase = t / fps * Math.PI * 2,
					Height = 1 - t / count,
				})
				.Select(state =>
				{
					var x = (float)Math.Sin(state.Phase * frequency.X) * amplitude.X * state.Height;
					var y = (float)Math.Sin(state.Phase * frequency.Y) * amplitude.Y * state.Height;
					return center + new Vector2DF(x, y);
				});
		}

		/// <summary>
		/// シーケンスを時間単位として、時間によって振動し続けるベクトルの値が流れるシーケンスに変換します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">値が通ることで時間を進めるシーケンス。</param>
		/// <param name="center">振動の中心点。</param>
		/// <param name="amplitude">軸ごとの振幅。</param>
		/// <param name="frequency">軸ごとの周波数。</param>
		/// <returns></returns>
		public static IObservable<Vector2DF> Wiggle<T>(this IObservable<T> source, Vector2DF center, Vector2DF amplitude, Vector2DF frequency)
		{
			var fps = Engine.TargetFPS;
			return source.Select((x, i) => (float)i)
				.Select(t => t / fps * Math.PI * 2)
				.Select(state =>
				{
					var x = (float)Math.Sin(state * frequency.X) * amplitude.X;
					var y = (float)Math.Sin(state * frequency.Y) * amplitude.Y;
					return center + new Vector2DF(x, y);
				});
		}
	}
}
