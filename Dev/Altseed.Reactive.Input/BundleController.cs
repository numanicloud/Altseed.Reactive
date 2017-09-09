using System.Collections.Generic;
using System.Linq;

namespace Altseed.Reactive.Input
{
	/// <summary>
	/// 複数のコントローラ入力を統合するコントローラ。
	/// </summary>
	/// <typeparam name="TAbstractKey">操作を表す型。</typeparam>
	public class BundleController<TAbstractKey> : Controller<TAbstractKey>
	{
		List<Controller<TAbstractKey>> controllers;

		/// <summary>
		/// なんらかの入力に対応付けられている操作のコレクションを取得します。
		/// </summary>
		public override IEnumerable<TAbstractKey> Keys
		{
			get { return controllers.SelectMany(x => x.Keys).ToArray(); }
		}

		/// <summary>
		/// このコントローラに登録されているコントローラを自動で更新するかどうかを表す真偽値。
		/// </summary>
		public bool IsChildrenUpdated { get; set; } = true;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BundleController(params Controller<TAbstractKey>[] controllers)
		{
			this.controllers = controllers.ToList();
		}

		/// <summary>
		/// このコントローラに指定したコントローラを統合します。
		/// </summary>
		/// <param name="controller">このコントローラに統合するコントローラ。</param>
		public void AddController(Controller<TAbstractKey> controller)
		{
			controllers.Add(controller);
		}

		/// <summary>
		/// 指定した操作に対応する入力の状態を取得します。
		/// </summary>
		/// <param name="key">入力の状態を取得する操作。</param>
		/// <returns></returns>
		public override InputState? GetState(TAbstractKey key)
		{
			return Merge(controllers.Select(x => x.GetState(key)).ToArray());
		}

		/// <summary>
		/// コントローラの状態を更新します。
		/// </summary>
		public override void Update()
		{
			if (IsChildrenUpdated)
			{
				foreach (var item in controllers)
				{
					item.Update();
				}
			}
		}

		private InputState? Merge(InputState?[] states)
		{
			if (states.Any(x => x == InputState.Hold))
			{
				return InputState.Hold;
			}
			else if (states.Any(x => x == InputState.Push))
			{
				return InputState.Push;
			}
			else if (states.Any(x => x == InputState.Release))
			{
				return InputState.Release;
			}
			else if(states.Any(x => x == InputState.Free))
			{
				return InputState.Free;
			}
			else
			{
				return null;
			}
		}
	}
}
