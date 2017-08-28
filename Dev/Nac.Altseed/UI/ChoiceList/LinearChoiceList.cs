using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Nac.Altseed.Linq;

namespace Nac.Altseed.UI.ChoiceList
{
	public enum LinearChoiceListControl
	{
		Previous, Next
	}


	/// <summary>
	/// 一方向に並ぶ選択肢に関する操作を提供するクラス。
	/// </summary>
	/// <typeparam name="TAbstractKey"></typeparam>
	public class LinearChoiceList<TChoice> : IChoiceList<TChoice, LinearChoiceListControl>
	{
		/// <summary>
		/// 選択肢が0個の場合に選択されている位置のインデックス。
		/// </summary>
		public static readonly int Disabled = -1;
		
		private int size_;
		private int selectedIndex_;
		private List<TChoice> choices_;

		public IEnumerable<TChoice> Choices => choices_;

		/// <summary>
		/// 選択肢の項目数を取得または設定します。
		/// </summary>
		public int Size
		{
			get { return size_; }
			private set
			{
				if (value < 0)
				{
					throw new Exception("Sizeは0以上である必要があります。");
				}

				size_ = value;

				int prev = SelectedIndex;
				if (size_ == 0)
				{
					SelectedIndex = Disabled;
				}
				else if (SelectedIndex == Disabled)
				{
					SelectNextIndex();
				}
				else if (SelectedIndex >= size_)
				{
					SelectPreviousIndex();
				}
			}
		}

		/// <summary>
		/// 現在選択されているインデックスを取得または設定します。
		/// </summary>
		/// <remarks>こちらで選択されているインデックスを操作すると、ループやスキップを考慮しません。</remarks>
		public int SelectedIndex
		{
			get { return selectedIndex_; }
			set
			{
				var prev = selectedIndex_;
				selectedIndex_ = value;
				if (selectedIndex_ < 0)
				{
					selectedIndex_ = 0;
				}
				if (selectedIndex_ >= Size) // Size = 0 のときは -1 に戻す
				{
					selectedIndex_ = Size - 1;
				}
			}
		}
		
		/// <summary>
		/// 選択肢間の移動操作をループできるようにするかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool Loop { get; set; }

		public TChoice SelectedChoice => SelectedIndex != Disabled
			? choices_[SelectedIndex]
			: throw new InvalidOperationException("選択肢がなにも登録されていません。");

		public int DisabledIndex => Disabled;


		/// <summary>
		/// 初期値を使用して、LinearChoiceList の新しいインスタンスを生成します。
		/// </summary>
		public LinearChoiceList()
		{
			Size = 0;
			choices_ = new List<TChoice>();
		}

		public void AddChoice(TChoice choice)
		{
			choices_.Add(choice);
			Size++;
		}

		public bool RemoveChoice(TChoice choice)
		{
			// リスト内に選択肢があれば削除に進む
			var index = choices_.IndexOf(x => x.Equals(choice));
			if (index != -1)
			{
				choices_.RemoveAt(index);
				// 選択中のものより前の選択肢を消したなら位置をずらす
				if(index <= SelectedIndex)
				{
					SelectedIndex--;
				}
				Size--;
				return true;
			}
			return false;
		}

		public void InsertChoice(int index, TChoice choice)
		{
			choices_.Insert(index, choice);
			Size++;
			if (index <= SelectedIndex)
			{
				SelectedIndex++;
			}
		}

		public void	ClearChoice()
		{
			choices_.Clear();
			Size = 0;
		}

		/// <summary>
		/// 選択中のインデックスをひとつ前へ移動します。
		/// </summary>
		/// <returns>移動に成功したか</returns>
		public bool SelectPreviousIndex()
		{
			if (Size == 0)
			{
				return false;
			}

			var range = Enumerable.Range(0, Size);
			var choices = range.Take(SelectedIndex);
			if (Loop)
			{
				choices = range.Skip(SelectedIndex).Concat(choices);
			}

			if (choices.Any())
			{
				selectedIndex_ = choices.Last();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 選択中のインデックスをひとつ次へ移動します。
		/// </summary>
		public bool SelectNextIndex()
		{
			if (Size == 0)
			{
				return true;
			}

			var range = Enumerable.Range(0, Size);
			var choices = range.Skip(SelectedIndex + 1);
			if (Loop)
			{
				choices = choices.Concat(range.Take(SelectedIndex + 1));
			}

			if (choices.Any())
			{
				selectedIndex_ = choices.First();
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 選択中の選択肢を強制的に変更します。
		/// </summary>
		/// <param name="choice">新しく選択する選択肢。</param>
		/// <returns>選択肢が変更されたかどうかを表す真偽値。</returns>
		public bool SetSelectedChoice(TChoice choice)
		{
			var prev = SelectedIndex;
			SelectedIndex = choices_.IndexOf(choice);
			return prev != SelectedIndex;
		}

		/// <summary>
		/// 指定した操作に応じて選択を移動します。
		/// </summary>
		/// <param name="control">選択を動かす方向などを表す値。</param>
		/// <returns>選択肢が変更されたかどうかを表す真偽値。</returns>
		public bool MoveSelection(LinearChoiceListControl control)
		{
			switch (control)
			{
			case LinearChoiceListControl.Previous:
				return SelectPreviousIndex();

			case LinearChoiceListControl.Next:
				return SelectNextIndex();
			}
			return false;
		}
	}
}
