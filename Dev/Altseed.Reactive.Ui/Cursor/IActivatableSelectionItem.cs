using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// カーソルが当たると状態を変える選択肢オブジェクトのインターフェース。
	/// </summary>
	public interface IActivatableSelectionItem
	{
		/// <summary>
		/// この選択肢を選択状態にします。
		/// </summary>
		void Activate();
		/// <summary>
		/// この選択肢を非選択状態にします。
		/// </summary>
		void Disactivate();
	}
}
