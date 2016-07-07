using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.UI.Selector
{
	/// <summary>
	/// 選択UIのインターフェース。
	/// </summary>
	/// <typeparam name="TChoice">選択の結果として返す値の型。</typeparam>
	public interface IReadOnlySelector<out TChoice>
	{
		/// <summary>
		/// この選択UIが操作中(アクティブ)であるかどうかの真偽値を取得します。
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// この選択UIに登録されている選択肢のコレクションを取得します。
		/// </summary>
		IEnumerable<TChoice> AvailableChoices { get; }

		/// <summary>
		/// 選択中の選択肢のインデックスを取得または設定します。
		/// </summary>
		int SelectedIndex { get; }

		/// <summary>
		/// 選択中の選択肢を取得します。
		/// </summary>
		TChoice SelectedChoice { get; }

		/// <summary>
		/// 選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		IObservable<TChoice> OnSelectionChanged { get; }

		/// <summary>
		/// ユーザーの入力によって選択が移動したことを通知するイベントを取得または設定します。
		/// </summary>
		IObservable<TChoice> OnMove { get; }

		/// <summary>
		/// 選択が確定されたことを通知するイベントを取得または設定します。
		/// </summary>
		IObservable<TChoice> OnDecide { get; }

		/// <summary>
		/// 選択がキャンセルされたことを通知するイベントを取得または設定します。
		/// </summary>
		IObservable<TChoice> OnCancel { get; }

		/// <summary>
		/// この選択UIのアクティブ状態が変化した時に通知するイベントを取得します。
		/// </summary>
		IObservable<bool> OnActivationStateChanged { get; }
	}
}
