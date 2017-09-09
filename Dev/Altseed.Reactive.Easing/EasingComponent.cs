using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Easings
{
	/// <summary>
	/// イージングによるアニメーションを2Dオブジェクトに適用するコンポーネントを提供します。
	/// </summary>
	public class EasingComponent : Object2DComponent
	{
		private IEasing easing;
		private Action<Object2D, float> application;
		private int maxFrame;
		private float start;
		private float goal;
		private int frames;

		/// <summary>
		/// 任意のイージング設定とアニメーションする値を実際に適用するデリゲートを指定して、EasingComponent の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="easing">アニメーションに用いるイージング設定。</param>
		/// <param name="application">アニメーションする値を実際にオブジェクトに適用するデリゲート。この引数に渡すために <see cref="ApplyPositionX(Object2D, float)"/> 静的メソッドなども用意されています。</param>
		/// <param name="maxFrame">アニメーションにかける時間。</param>
		/// <param name="start">アニメーションの始点。</param>
		/// <param name="goal">アニメーションの終点。</param>
		public EasingComponent(IEasing easing, Action<Object2D, float> application, int maxFrame, float start, float goal)
		{
			this.easing = easing;
			this.application = application;
			this.maxFrame = maxFrame;
			this.start = start;
			this.goal = goal;
			frames = 0;
		}

		protected override void OnUpdate()
		{
			application(Owner, easing.GetGeneralValue(frames, maxFrame, start, goal));
			++frames;
			if (frames > maxFrame)
			{
				Dispose();
			}
		}

		/// <summary>
		/// アニメーションの値をオブジェクトのX座標に適用する関数。EasingComponent のコンストラクタに引数として渡して使用します。
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void ApplyPositionX(Object2D obj, float value)
		{
			obj.Position = obj.Position.WithX(value);
		}

		/// <summary>
		/// アニメーションの値をオブジェクトのY座標に適用する関数。EasingComponent のコンストラクタに引数として渡して使用します。
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void ApplyPositionY(Object2D obj, float value)
		{
			obj.Position = obj.Position.WithY(value);
		}

		/// <summary>
		/// アニメーションの値をオブジェクトの不透明度に適用する関数。EasingComponent のコンストラクタに引数として渡して使用します。
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void ApplyColorAlpha(Object2D obj, float value)
		{
			if (obj is DrawnObject2D drawn)
			{
				drawn.Color = drawn.Color.WithAlpha((byte)value);
			}
		}
	}
}
