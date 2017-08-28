using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.UI.ChoiceList
{
	public interface IChoiceList<TChoice, TControl>
	{
		/// <summary>
		/// 現在選択中の選択肢を取得または設定します。
		/// </summary>
		TChoice SelectedChoice { get; }

		/// <summary>
		/// 選択中の選択肢を強制的に変更します。
		/// </summary>
		/// <param name="choice">新しく選択する選択肢。</param>
		/// <returns>選択肢が変更されたかどうかを表す真偽値。</returns>
		bool SetSelectedChoice(TChoice choice);

		/// <summary>
		/// 指定した操作に応じて選択を移動します。
		/// </summary>
		/// <param name="control">選択を動かす方向などを表す値。</param>
		/// <returns>選択肢が変更されたかどうかを表す真偽値。</returns>
		bool MoveSelection(TControl control);
	}
}
