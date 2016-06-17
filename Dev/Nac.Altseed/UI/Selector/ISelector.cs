using System;
using asd;
using Nac.Altseed.UI.Selector;

namespace Nac.Altseed.UI
{
	/// <summary>
	/// カスタマイズ操作を適用できる選択UIのインターフェース。
	/// </summary>
	/// <typeparam name="TChoice">選択の結果として返す値の型。</typeparam>
	public interface ISelector<TChoice> : IReadOnlySelector<TChoice>
	{
		/// <summary>
		/// この選択UIが操作中(アクティブ)であるかどうかの真偽値を取得または設定します。
		/// </summary>
		new bool IsActive { get; set; }

		/// <summary>
		/// UIに選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="item">追加する選択肢を表示する2Dオブジェクト。</param>
		void AddChoice(TChoice choice, Object2D item);

		/// <summary>
		/// UIから選択肢を削除します。
		/// </summary>
		/// <param name="choice">削除する選択肢。</param>
		Object2D RemoveChoice(TChoice choice);

		/// <summary>
		/// UIに選択肢を挿入します。
		/// </summary>
		/// <param name="index">挿入する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="item">挿入する選択肢を表示する2Dオブジェクト。</param>
		void InsertChoice(int index, TChoice choice, Object2D item);

		/// <summary>
		/// UIからすべての選択肢を削除します。
		/// </summary>
		void ClearChoice();
    }

	public interface ISelector<TChoice, in TAbstractKey> : ISelector<TChoice>
	{
		void BindKey(TAbstractKey next, TAbstractKey prev, TAbstractKey decide, TAbstractKey cancel);
	}
}
