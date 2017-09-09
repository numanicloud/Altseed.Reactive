using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.UI.ChoiceList
{
	interface IBoundChoiceList<TChoice>
	{
		/// <summary>
		/// 現在選択中の選択肢を取得または設定します。
		/// </summary>
		TChoice SelectedChoice { get; }
	}
}
