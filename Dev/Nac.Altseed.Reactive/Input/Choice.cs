using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Nac.Altseed.Reactive.Input
{
    /// <summary>
    /// 選択肢に対する操作を表す列挙体。
    /// </summary>
	public enum ChoiceControl
	{
		Next, Previous, Decide, Cancel
	}

	/// <summary>
	/// キー入力によって選択肢の処理をするクラス。
	/// </summary>
	/// <typeparam name="TKeyCode">キーの識別子となる型。</typeparam>
	public class Choice<TAbstractKey>
	{
		public static readonly int DisabledIndex = -1;

		private int size_, selectedIndex_;
		private Controller<TAbstractKey> controller { get; set; }
		private Dictionary<TAbstractKey, ChoiceControl> controlls { get; set; }
		private IList<int> skippedIndex { get; set; }

        private Subject<int> onSelectionChanged_ { get; set; } = new Subject<int>();
        private Subject<int> onMove_ { get; set; } = new Subject<int>();
        private Subject<int> onDecide_ { get; set; } = new Subject<int>();
        private Subject<int> onCancel_ { get; set; } = new Subject<int>();

		/// <summary>
		/// 選択肢の項目数を取得または設定します。
		/// </summary>
		public int Size
		{
			get { return size_; }
			set
			{
				if (value < 0)
				{
					throw new Exception("Sizeは0以上である必要があります。");
				}

				size_ = value;

				int prev = SelectedIndex;
				if(size_ == 0)
				{
					SelectedIndex = DisabledIndex;
				}
				else if(SelectedIndex == DisabledIndex)
				{
					SelectNextIndex();
				}
				else if(SelectedIndex >= size_)
				{
					SelectPreviousIndex();
				}

				if(prev != SelectedIndex)
				{
					onSelectionChanged_.OnNext(SelectedIndex);
				}
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
				var prev = selectedIndex_;
                selectedIndex_ = value;
				if(selectedIndex_ < 0)
				{
					selectedIndex_ = 0;
				}
				if(selectedIndex_ >= Size)	// Size = 0 のときは -1 に戻す
				{
					selectedIndex_ = Size - 1;
				}
				if(skippedIndex.Contains(selectedIndex_))
				{
					throw new ArgumentException("スキップされるよう設定されている選択肢は指定できません。");
				}
				if(prev != selectedIndex_)
				{
                    onSelectionChanged_.OnNext(selectedIndex_);
				}
			}
		}
        /// <summary>
        /// 選択肢間の移動操作をループできるようにするかどうかの真偽値を取得または設定します。
        /// </summary>
		public bool Loop { get; set; }
		/// <summary>
		/// 登録したコントローラーを自動的にUpdateするかどうかの真偽値を取得または設定します。
		/// </summary>
		public bool IsControllerUpdated { get; set; }

        public IObservable<int> OnSelectionChanged => onSelectionChanged_;
        public IObservable<int> OnMove => onMove_;
        public IObservable<int> OnDecide => onDecide_;
        public IObservable<int> OnCancel => onCancel_;

        /// <summary>
        /// Choiceクラスを生成します。
        /// </summary>
        /// <param name="size">選択肢の数。</param>
        /// <param name="loop">選択肢間の移動をループさせるかどうか。</param>
        /// <param name="keyIsHold">特定のキーが押下されているかどうかを取得するデリゲート。</param>
		public Choice(int size, Controller<TAbstractKey> controller)
		{
			this.controller = controller;
			controlls = new Dictionary<TAbstractKey, ChoiceControl>();
			skippedIndex = new List<int>();
			Size = size;
			Loop = false;
            SelectedIndex = 0;
		}

        /// <summary>
        /// 指定したキーを指定した選択肢の操作に割り当てます。
        /// </summary>
        /// <param name="key">操作に割り当てるキー。</param>
        /// <param name="controll">キーに割り当てる操作。</param>
		/// <remarks>同じキーに異なる操作を割り当てると、最後の割り当てで上書きされます。</remarks>
		public void BindKey(TAbstractKey key, ChoiceControl controll)
		{
			controlls[key] = controll;
		}

		/// <summary>
		/// 指定したキーへの選択肢の操作の割り当てを解除します。
		/// </summary>
		/// <param name="key">割り当てを解除するキー。</param>
		public void DisbindKey(TAbstractKey key)
		{
			controlls.Remove(key);
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
			skippedIndex.Add(index);

            if(skippedIndex.Contains(SelectedIndex))
            {
				var prev = SelectedIndex;
				var successToMove = SelectNextIndex() || SelectPreviousIndex();
				if(!successToMove)
				{
					SelectedIndex = DisabledIndex;
				}
				onSelectionChanged_.OnNext(SelectedIndex);
			}
		}

		/// <summary>
		/// 選択肢の移動の際にスキップする選択肢のインデックスを削除します。
		/// </summary>
		/// <param name="index">スキップしないようにする選択肢のインデックス。</param>
		public void RemoveSkippedIndex(int index)
		{
			if(index >= Size || index < 0)
			{
				throw new ArgumentException("indexの範囲が不正です index=" + index, "index");
			}
			skippedIndex.Remove(index);

			if(SelectedIndex == DisabledIndex)
			{
				var successToMove = SelectNextIndex();
				if(successToMove)
				{
					onSelectionChanged_.OnNext(SelectedIndex);
				}
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
					ChoiceControl controll = controlls[item];
					switch (controll)
					{
						case ChoiceControl.Next:
							MoveNext();
							break;
						case ChoiceControl.Previous:
							MovePrevious();
							break;
						case ChoiceControl.Decide:
							Decide();
							break;
						case ChoiceControl.Cancel:
							Cancel();
							break;
					}
                    break;
				}
			}
		}


		private bool SelectNextIndex()
		{
			if(Size == 0)
			{
				return false;
			}
			
			var range = Enumerable.Range(0, Size);
			var choices = range.Skip(SelectedIndex + 1);
			if(Loop)
			{
				choices = choices.Concat(range.Take(SelectedIndex + 1));
			}
			choices = choices.Except(skippedIndex);

			if(choices.Any())
			{
				selectedIndex_ = choices.First();
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool SelectPreviousIndex()
		{
			if(Size == 0)
			{
				return false;
			}

			var range = Enumerable.Range(0, Size);
			var choices = range.Take(SelectedIndex);
			if(Loop)
			{
				choices = range.Skip(SelectedIndex).Concat(choices);
			}
			choices = choices.Except(skippedIndex);

			if(choices.Any())
			{
				selectedIndex_ = choices.Last();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void MoveNext()
		{
			if(Size == 0)
			{
				return;
			}
			
			var result = SelectNextIndex();
			if(result)
			{
                onMove_.OnNext(SelectedIndex);
				onSelectionChanged_.OnNext(SelectedIndex);
			}
		}

		private void MovePrevious()
		{
			if(Size == 0)
			{
				return;
			}
			
			var result = SelectPreviousIndex();
			if(result)
            {
                onMove_.OnNext(SelectedIndex);
                onSelectionChanged_.OnNext(SelectedIndex);
            }
		}

		private void Decide()
		{
			if(Size - skippedIndex.Count == 0)
			{
				return;
			}

            onDecide_.OnNext(SelectedIndex);
		}

		private void Cancel()
		{
            onCancel_.OnNext(SelectedIndex);
		}
	}
}
