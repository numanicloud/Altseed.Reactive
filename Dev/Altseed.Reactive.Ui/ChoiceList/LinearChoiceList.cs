using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Altseed.Reactive.Helper;

namespace Altseed.Reactive.Ui.ChoiceList
{
	/// <summary>
	/// 一方向に並ぶ選択肢に関する操作を提供するクラス。
	/// </summary>
	/// <typeparam name="TChoice">選択肢の型。</typeparam>
	public class LinearChoiceList<TChoice>
	{
		/// <summary>
		/// 選択肢が0個の場合に選択されている位置のインデックス。
		/// </summary>
		public static readonly int Disabled = -1;
		
		private int size_;
		private int selectedIndex_;
		private List<TChoice> choices_;
		private List<int> skipped_;

		/// <summary>
		/// 登録されている選択肢のコレクション。
		/// </summary>
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

		/// <summary>
		/// 現在選択されている選択肢を取得します。
		/// </summary>
		public TChoice SelectedChoice => SelectedIndex != Disabled
			? choices_[SelectedIndex]
			: throw new InvalidOperationException("選択肢がなにも登録されていません。");

		/// <summary>
		/// 何も選択されていないときに <see cref="SelectedIndex"/> に設定される値。
		/// </summary>
		public int DisabledIndex => Disabled;


		/// <summary>
		/// 初期値を使用して、LinearChoiceList の新しいインスタンスを生成します。
		/// </summary>
		public LinearChoiceList()
		{
			Size = 0;
			choices_ = new List<TChoice>();
		}

		/// <summary>
		/// 選択肢を追加します。
		/// </summary>
		/// <param name="choice">追加する選択肢。</param>
		/// <param name="skipped">この選択肢をスキップするかどうかを表す真偽値。</param>
		public void AddChoice(TChoice choice, bool skipped = false)
		{
			choices_.Add(choice);
			Size++;
			if (skipped)
			{
				skipped_.Add(choices_.Count - 1);
			}
		}

		/// <summary>
		/// 選択肢を削除します。
		/// </summary>
		/// <param name="choice">削除する選択肢。</param>
		/// <returns>選択肢が削除されたかどうかを表す真偽値。追加されていないものを削除しようとすると<c>false</c>になります。</returns>
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
					SelectPreviousIndex();
				}
				Size--;
				skipped_.Remove(index);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 選択肢を挿入します。
		/// </summary>
		/// <param name="index">挿入する位置。</param>
		/// <param name="choice">挿入する選択肢。</param>
		/// <param name="skipped">この選択肢がスキップするかどうかを表す真偽値。</param>
		public void InsertChoice(int index, TChoice choice, bool skipped = false)
		{
			choices_.Insert(index, choice);
			Size++;
			if (index <= SelectedIndex)
			{
				SelectNextIndex();
			}
			if (skipped)
			{
				skipped_.Add(index);
			}
		}

		/// <summary>
		/// 選択肢をクリアします。
		/// </summary>
		public void	ClearChoice()
		{
			choices_.Clear();
			Size = 0;
			skipped_.Clear();
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
			choices = choices.Except(skipped_);

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
			choices = choices.Except(skipped_);

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
	}
}
