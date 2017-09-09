using System.Collections.Generic;

namespace Altseed.Reactive.Input
{
	/// <summary>
	/// キーの押しっぱなしを連続入力とみなすコントローラ。
	/// </summary>
	/// <typeparam name="TAbstract">操作を表す型。</typeparam>
	public class SteppingController<TAbstract> : Controller<TAbstract>
	{
		class HoldManager
		{
			private int inputTime;
			private int startTime;
			private int span;

			public HoldManager(int startTime, int span)
			{
				this.startTime = startTime;
				this.span = span;
				inputTime = 0;
			}

			public void Update(InputState? state)
			{
				if(state == InputState.Push || state == InputState.Hold)
				{
					inputTime++;
				}
				else
				{
					inputTime = 0;
				}
			}

			public bool IsPush()
			{
				return (inputTime - startTime) % span == 1;
			}
		}

		Controller<TAbstract> controller;
		Dictionary<TAbstract, HoldManager> managers;

		/// <summary>
		/// なんらかの入力に対応付けられている操作のコレクションを取得します。
		/// </summary>
		public override IEnumerable<TAbstract> Keys
		{
			get { return controller.Keys; }
		}

		/// <summary>
		/// このコントローラに登録されているコントローラを自動で更新するかどうかを表す真偽値。
		/// </summary>
		public bool IsChildUpdated { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SteppingController(Controller<TAbstract> controller)
		{
			this.controller = controller;
			managers = new Dictionary<TAbstract, HoldManager>();
		}

		/// <summary>
		/// 指定した操作が押しっぱなしにされたときに連続入力とみなすように設定します。
		/// </summary>
		/// <param name="key">押しっぱなしを連続入力とみなす操作。</param>
		/// <param name="startTime">押しっぱなしと判定されるまでのフレーム数。</param>
		/// <param name="span">連続入力の間隔(フレーム)。</param>
		public void EnableSteppingHold(TAbstract key, int startTime, int span)
		{
			managers[key] = new HoldManager(startTime, span);
		}

		/// <summary>
		/// 指定した操作に対応する入力の状態を取得します。
		/// </summary>
		/// <param name="key">入力の状態を取得する操作。</param>
		/// <returns></returns>
		public override InputState? GetState(TAbstract key)
		{
			if(!managers.ContainsKey(key))
			{
				return controller.GetState(key);
			}
			else
			{
				if(managers[key].IsPush())
				{
					return InputState.Push;
				}
				else
				{
					return controller.GetState(key);
				}
			}
		}

		/// <summary>
		/// コントローラの状態を更新します。
		/// </summary>
		public override void Update()
		{
			foreach(var item in managers)
			{
				item.Value.Update(controller.GetState(item.Key));
			}
			if(IsChildUpdated)
			{
				controller.Update();
			}
		}
	}
}
