using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.Linq
{
	/// <summary>
	/// Altseed のオブジェクトシステムに関するリアクティブ メソッドを提供するヘルパー。
	/// </summary>
	public static class ObjectAction
	{
		/// <summary>
		/// シーケンスを時間単位としてオブジェクトをなめらかに移動させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">移動させるオブジェクト。</param>
		/// <param name="goal">終点の位置。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">移動にかける時間。</param>
		/// <returns></returns>
		public static IObservable<Vector2DF> DoEasingPosition<T>(this IObservable<T> source, Object2D obj, Vector2DF goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => obj.Position)
				.EasingVector2DF(goal, start, end, count)
				.Do(p => obj.Position = p);
		}

		/// <summary>
		/// シーケンスを時間単位としてオブジェクトをX軸方向になめらかに移動させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">移動させるオブジェクト。</param>
		/// <param name="goal">終点のX座標。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">移動にかける時間。</param>
		/// <returns></returns>
		public static IObservable<float> DoEasingX<T>(this IObservable<T> source, Object2D obj, float goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => obj.Position.X)
				.EasingValue(goal, start, end, count)
				.Do(x => obj.Position = obj.Position.WithX(x));
		}

		/// <summary>
		/// シーケンスを時間単位としてオブジェクトをY軸方向になめらかに移動させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">移動させるオブジェクト。</param>
		/// <param name="goal">終点のY座標。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">移動にかける時間。</param>
		/// <returns></returns>
		public static IObservable<float> DoEasingY<T>(this IObservable<T> source, Object2D obj, float goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => obj.Position.Y)
				.EasingValue(goal, start, end, count)
				.Do(y => obj.Position = obj.Position.WithY(y));
		}

		/// <summary>
		/// シーケンスを時間単位としてオブジェクトをなめらかに拡大・縮小します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">拡大・縮小するオブジェクト。</param>
		/// <param name="goal">終点の拡大率。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">拡大・縮小にかける時間。</param>
		/// <returns></returns>
		public static IObservable<Vector2DF> DoEasingScale<T>(this IObservable<T> source, Object2D obj, Vector2DF goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => obj.Scale)
				.EasingVector2DF(goal, start, end, count)
				.Do(p => obj.Scale = p);
		}

		/// <summary>
		/// シーケンスを時間単位としてオブジェクトをなめらかに回転させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">回転させるオブジェクト。</param>
		/// <param name="goal">終点の角度。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">回転にかける時間。</param>
		/// <returns></returns>
		public static IObservable<float> DoEasingAngle<T>(this IObservable<T> source, Object2D obj, float goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => obj.Angle)
				.EasingValue(goal, start, end, count)
				.Do(a => obj.Angle = a);
		}

		/// <summary>
		/// シーケンスを時間単位として、オブジェクトのα値をなめらかに変化させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">α値を変化させるオブジェクト。</param>
		/// <param name="goal">終点のα値。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">α値の変化にかける時間。</param>
		/// <returns></returns>
		public static IObservable<byte> DoEasingAlpha<T>(this IObservable<T> source, TextureObject2D obj, byte goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => (float)obj.Color.A)
				.EasingValue(goal, start, end, count)
				.Select(a => (byte)a)
				.Do(a => obj.Color = new Color(obj.Color.R, obj.Color.G, obj.Color.B, a));
		}

		/// <summary>
		/// シーケンスを時間単位として、オブジェクトのα値をなめらかに変化させます。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">時間単位となるシーケンス。</param>
		/// <param name="obj">α値を変化させるオブジェクト。</param>
		/// <param name="goal">終点のα値。</param>
		/// <param name="start">始点での速度。</param>
		/// <param name="end">終点での速度。</param>
		/// <param name="count">α値の変化にかける時間。</param>
		/// <returns></returns>
		public static IObservable<byte> DoEasingAlpha<T>(this IObservable<T> source, TextObject2D obj, byte goal, EasingStart start, EasingEnd end, int count)
		{
			return source.Select(t => (float)obj.Color.A)
				.EasingValue(goal, start, end, count)
				.Select(a => (byte)a)
				.Do(a => obj.Color = new Color(obj.Color.R, obj.Color.G, obj.Color.B, a));
		}

		/// <summary>
		/// INotifyUpdated の OnUpdateEvent イベントを数えるタイマーとなるシーケンスを生成します。指定した実時間に達するとシーケンスが完了します。
		/// </summary>
		/// <param name="source">更新時間を流す INotifyUpdated のインスタンス。</param>
		/// <param name="time">完了するまでの時間。</param>
		/// <returns></returns>
		public static IObservable<float> TickingTimerForRealTime(this INotifyUpdated source, float time)
		{
			return source.OnUpdateEvent.Total().TakeWhile(f => f < time);
		}

		/// <summary>
		/// INotifyUpdated の OnUpdateEvent イベントを数えるタイマーとなるシーケンスを生成します。指定した回数に達するとシーケンスが完了します。
		/// </summary>
		/// <param name="source">時間単位となる INotifyUpdated のインスタンス。</param>
		/// <param name="count">完了するまでの回数。</param>
		/// <returns></returns>
		public static IObservable<int> TickingTimer(this INotifyUpdated source, int count)
		{
			return source.OnUpdateEvent.Select((x, i) => i).TakeWhile(t => t < count);
		}

		/// <summary>
		/// INotifyUpdated の更新に基いて数えた実時間を引数に、指定したデリゲートを呼び出します。
		/// </summary>
		/// <param name="source">更新時間を流す INotifyUpdated のインスタンス。</param>
		/// <param name="action">更新のたびに呼び出されるデリゲート。引数に経過時間が渡されます。</param>
		/// <returns></returns>
		public static IObservable<float> TimeAnimation(this INotifyUpdated source, Action<float> action)
		{
			return source.OnUpdateEvent.Total().Do(action);
		}
	}
}
