using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Altseed.Reactive.Helper
{
	public static class LinqHelper
	{
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
		/// 流れてきた値を累計して後続に流します。
		/// </summary>
		/// <param name="source">累計される値のシーケンス。</param>
		/// <returns></returns>
		public static IObservable<long> Total(this IObservable<long> source)
		{
			long total = 0;
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

		public static IObservable<T> Append<T>(this IObservable<T> source, T value)
		{
			return source.Concat(Observable.Return(value));
		}

		/// <summary>
		/// コレクションの要素から指定した条件を最初に満たすオブジェクトを検索し、そのインデックスを返します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">検索対象となるコレクション。</param>
		/// <param name="predicate">検索する要素の条件を定義するデリゲート。</param>
		/// <returns></returns>
		public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			int index = 0;
			foreach (var item in source)
			{
				if (predicate(item))
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		/// <summary>
		/// 指定した文字のコレクションを連結してひとつの文字列として返します。
		/// </summary>
		/// <param name="source">文字のコレクション。</param>
		/// <returns>連結した文字列。</returns>
		public static string Concat(this IEnumerable<char> source)
		{
			return string.Concat(source);
		}
	}
}
