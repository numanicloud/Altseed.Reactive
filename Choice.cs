using Nac.Altseed.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nac.Altseed
{
    /// <summary>
    /// 選択肢に対する操作を表す列挙体。
    /// </summary>
	public enum ChoiceControll
	{
		Next, Previous, Decide, Cancel
	}

	/// <summary>
	/// キー入力によって選択肢の処理をするクラス。
	/// </summary>
	/// <typeparam name="TKeyCode">キーの識別子となる型。</typeparam>
	public class Choice<TAbstractKey>
	{
		int size_, selectedIndex_;
		
		Controller<TAbstractKey> controller { get; set; }
		Dictionary<TAbstractKey, ChoiceControll> controlls { get; set; }
		IList<int> skippedIndex { get; set; }

		/// <summary>
		/// 選択肢の項目数を取得または設定します。
		/// </summary>
		public int Size
		{
			get { return size_; }
			set
			{
				if (value <= 0)
				{
					throw new Exception("Sizeは1以上である必要があります。");
				}
				size_ = value;
				if (SelectedIndex >= size_)
					SelectedIndex = size_ - 1;
			}
		}
        /// <summary>
        /// 現在選択されている項目のインデックスを取得します。
        /// </summary>
		public int SelectedIndex
		{
			get { return selectedIndex_; }
			set
			{
				selectedIndex_ = value;
				if(selectedIndex_ < 0)
				{
					selectedIndex_ = 0;
				}
				else if(selectedIndex_ >= Size)
				{
					selectedIndex_ = Size - 1;
				}
				if(skippedIndex.Contains(selectedIndex_))
				{
					throw new ArgumentException("スキップされるよう設定されている選択肢は指定できません。");
				}
			}
		}
        /// <summary>
        /// 選択肢間の移動操作をループできるようにするかどうかの真偽値を取得または設定します。
        /// </summary>
		public bool Loop { get; set; }
		public bool IsControllerUpdated { get; set; }

		/// <summary>
		/// 選択肢間の移動が起きたときに発生するイベント。第１引数は移動前のインデックス、第２引数は移動後のインデックス。
		/// </summary>
		public event Action<int, int> OnMove;
        /// <summary>
        /// 選択肢を決定したときに発生するイベント。引数は選択された選択肢のインデックス。
        /// </summary>
		public event Action<int> OnDecide;
        /// <summary>
        /// 選択肢をキャンセルしたときに発生するイベント。引数はキャンセル時に選択されていた選択肢のインデックス。
        /// </summary>
		public event Action<int> OnCancel;

        /// <summary>
        /// Choiceクラスを生成します。
        /// </summary>
        /// <param name="size">選択肢の数。</param>
        /// <param name="loop">選択肢間の移動をループさせるかどうか。</param>
        /// <param name="keyIsHold">特定のキーが押下されているかどうかを取得するデリゲート。</param>
		public Choice(int size, Controller<TAbstractKey> controller)
		{
			Size = size;
			this.controller = controller;
			Loop = false;
			controlls = new Dictionary<TAbstractKey, ChoiceControll>();
			skippedIndex = new List<int>();
            SelectedIndex = 0;
		}

        /// <summary>
        /// 指定したキーを指定した選択肢の操作に割り当てます。
        /// </summary>
        /// <param name="keycode">操作に割り当てるキー コード。</param>
        /// <param name="controll">キーに割り当てる操作。</param>
		public void BindKey(TAbstractKey keycode, ChoiceControll controll)
		{
			controlls[keycode] = controll;
		}

        /// <summary>
        /// 選択肢の移動の際にスキップする選択肢のインデックスを追加します。
        /// </summary>
        /// <param name="index">スキップする選択肢のインデックス。</param>
		public void AddSkippedIndex(int index)
		{
			if (index >= Size || index < 0)
			{
				throw new ArgumentException("indexの範囲が不正です index=" + index, "index");
			}
			if (Size - 1 == skippedIndex.Count)
			{
				throw new InvalidOperationException("すべての選択肢をスキップするようには設定できません。");
			}
			skippedIndex.Add(index);
			
            while (skippedIndex.Contains(SelectedIndex))
            {
                ++SelectedIndex;
            }
		}

        /// <summary>
        /// 選択肢の制御を実行します。毎フレーム呼び出す必要があります。
        /// </summary>
		public void Update()
		{
			if (IsControllerUpdated)
			{
				controller.Update();
			}
			foreach (TAbstractKey item in controlls.Keys)
			{
				if (controller.GetState(item) == InputState.Push)
				{
					ChoiceControll controll = controlls[item];
					switch (controll)
					{
						case ChoiceControll.Next:
							MoveNext();
							break;
						case ChoiceControll.Previous:
							MovePrevious();
							break;
						case ChoiceControll.Decide:
							Decide();
							break;
						case ChoiceControll.Cancel:
							Cancel();
							break;
					}
				}
			}
		}

		private void MoveNext()
		{
			var prev = SelectedIndex;

			var range = Enumerable.Range(0, Size);
			var choices = range.Skip(SelectedIndex + 1);
			if (Loop)
			{
				choices = choices.Concat(range.Take(SelectedIndex + 1));
			}
			choices = choices.Except(skippedIndex);
			if (choices.Any())
			{
				SelectedIndex = choices.First();
				OnMove?.Invoke(prev, SelectedIndex);
			}
		}

		private void MovePrevious()
		{
			var prev = SelectedIndex;

			var range = Enumerable.Range(0, Size);
			var choices = range.Take(SelectedIndex);
			if (Loop)
			{
				choices = range.Skip(SelectedIndex).Concat(choices);
			}
			choices = choices.Except(skippedIndex);
			if (choices.Any())
			{
				SelectedIndex = choices.Last();
				OnMove?.Invoke(prev, SelectedIndex);
			}
		}

		private void Decide()
		{
			if (OnDecide != null)
				OnDecide(SelectedIndex);
		}

		private void Cancel()
		{
			if (OnCancel != null)
				OnCancel(SelectedIndex);
		}
	}
}
