using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Easings
{
	/// <summary>
	/// 複数のアニメーションを持つオブジェクトを自然にアニメーションさせる機能を提供します。
	/// </summary>
	public class EasingManagerComponent : Object2DComponent
	{
		/// <summary>
		/// アニメーションの設定を表します。
		/// </summary>
		public class Setting
		{
			public IEasing Easing { get; }
			public float MaxFrame { get; }
			public float Goal { get; }

			public Setting(IEasing easing, float maxFrame, float goal)
			{
				Easing = easing;
				MaxFrame = maxFrame;
				Goal = goal;
			}
		}

		private Subject<float> newValueSubject_;
		private Subject<float> updateTime_;
		private float currentValue;
		private IDisposable animation;
		
		/// <summary>
		/// アニメーションの設定を保持するディクショナリ。
		/// </summary>
		public Dictionary<string, Setting> EasingSettings { get; set; }
		/// <summary>
		/// アニメーションの新しい値が通知されるプロバイダー。
		/// </summary>
		public IObservable<float> NewValue => newValueSubject_;

		/// <summary>
		/// 初期値を指定して、EasingManagerComponent の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="initialValue">アニメーションの初期値。</param>
		public EasingManagerComponent(float initialValue)
		{
			newValueSubject_ = new Subject<float>();
			updateTime_ = new Subject<float>();
			currentValue = initialValue;
			EasingSettings = new Dictionary<string, Setting>();

			NewValue.Subscribe(x => currentValue = x);
		}

		protected override void OnUpdate()
		{
			updateTime_.OnNext(Engine.DeltaTime);
		}

		/// <summary>
		/// ディクショナリに登録されたアニメーションを実行します。
		/// </summary>
		/// <param name="key">実行するアニメーションのキー。</param>
		public void Ease(string key)
		{
			Ease(EasingSettings[key]);
		}

		private void Ease(Setting setting)
		{
			animation?.Dispose();
			animation = updateTime_.Select((_, i) => (float)i)
				.TakeWhile(t => t < setting.MaxFrame)
				.Select(t => setting.Easing.GetGeneralValue(t, setting.MaxFrame, currentValue, setting.Goal))
				.Do(x => Console.WriteLine(x))
				.Subscribe(newValueSubject_.OnNext, () => animation = null);
		}
	}
}
