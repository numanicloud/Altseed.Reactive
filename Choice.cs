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
    /// キーのホールドを連続入力に変換する機能の設定。
    /// </summary>
	public class ChoiceHoldOption
	{
        /// <summary>
        /// キーをホールドしてから連続入力を始めるまでのフレーム数。
        /// </summary>
		public int HoldWaitTime { get; set; }
        /// <summary>
        /// キーをホールドしている間の連続入力の間隔のフレーム数。
        /// </summary>
		public int HoldSpanTime { get; set; }

        /// <summary>
        /// ChoiceHoldOptionを生成します。
        /// </summary>
        /// <param name="wait">キーをホールドしてから連続入力を始めるまでのフレーム数。</param>
        /// <param name="span">キーをホールドしている間の連続入力の間隔のフレーム数。</param>
		public ChoiceHoldOption(int wait, int span)
		{
			HoldWaitTime = wait;
			HoldSpanTime = span;
		}
	}

	/// <summary>
	/// キー入力によって選択肢の処理をするクラス。
	/// </summary>
	/// <typeparam name="TKeyCode">キーの識別子となる型。</typeparam>
	public class Choice<TKeyCode>
	{
		int size_;
		bool enabled;

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
				if (SelectedIndex > size_)
					SelectedIndex = size_;
			}
		}
        /// <summary>
        /// 現在選択されている項目のインデックスを取得します。
        /// </summary>
		public int SelectedIndex { get; private set; }
        /// <summary>
        /// 選択肢間の移動操作をループできるようにするかどうかの真偽値を取得または設定します。
        /// </summary>
		public bool Loop { get; set; }

        /// <summary>
        /// キーのホールドを連続した入力として扱う機能の設定を取得または設定します。nullの場合はキーのホールドを連続した入力として扱いません。
        /// </summary>
		public ChoiceHoldOption HoldOption { get; set; }
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

		Dictionary<TKeyCode, ChoiceControll> controlls { get; set; }
		Dictionary<ChoiceControll, int> count { get; set; }
		IList<int> skippedIndex { get; set; }
		Func<TKeyCode, bool> keyIsHold { get; set; }

        /// <summary>
        /// Choiceクラスを生成します。
        /// </summary>
        /// <param name="size">選択肢の数。</param>
        /// <param name="loop">選択肢間の移動をループさせるかどうか。</param>
        /// <param name="keyIsHold">特定のキーが押下されているかどうかを取得するデリゲート。</param>
		public Choice(int size, bool loop, Func<TKeyCode, bool> keyIsHold)
		{
			this.Size = size;
			this.Loop = loop;
			this.keyIsHold = keyIsHold;
			controlls = new Dictionary<TKeyCode, ChoiceControll>();
			count = new Dictionary<ChoiceControll, int>();
			count[ChoiceControll.Next] = 1;
			count[ChoiceControll.Previous] = 1;
			count[ChoiceControll.Decide] = 1;
			count[ChoiceControll.Cancel] = 1;
			enabled = false;
			skippedIndex = new List<int>();
            SelectedIndex = 0;
		}

        /// <summary>
        /// キーのホールドを連続入力として扱う機能の設定をして、Choiceクラスを生成します。
        /// </summary>
        /// <param name="size">選択肢の数。</param>
        /// <param name="loop">選択肢間の移動をループさせるかどうか。</param>
        /// <param name="keyIsHold">特定のキーが押下されているかどうかを取得するデリゲート。</param>
        /// <param name="holdWaitTime">キーをホールドしてから連続入力を始めるまでのフレーム数。</param>
        /// <param name="holdSpanTime">キーをホールドしている間の連続入力の間隔のフレーム数。</param>
		public Choice(int size, bool loop, Func<TKeyCode, bool> keyIsHold, int holdWaitTime, int holdSpanTime)
			: this(size, loop, keyIsHold)
		{
			HoldOption = new ChoiceHoldOption(holdWaitTime, holdSpanTime);
			enabled = false;
		}

        /// <summary>
        /// 指定したキーを指定した選択肢の操作に割り当てます。
        /// </summary>
        /// <param name="keycode">操作に割り当てるキー コード。</param>
        /// <param name="controll">キーに割り当てる操作。</param>
		public void BindKey(TKeyCode keycode, ChoiceControll controll)
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
			foreach (TKeyCode item in controlls.Keys)
			{
				ChoiceControll controll = controlls[item];

				if (keyIsHold(item)) ++count[controll];
				else count[controll] = 0;

				if (enabled &&
					(count[controll] == 1 ||
					HoldOption != null && ((count[controll] - HoldOption.HoldWaitTime) % HoldOption.HoldSpanTime == 1)))
				{
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

			if (!enabled)
			{
				var enums = Enum.GetValues(typeof(ChoiceControll))
					.Cast<ChoiceControll>()
					.Where(x => !controlls.ContainsValue(x));

				foreach (var item in enums)
				{
					count[item] = 0;
				}

				if (count.All(x => x.Value == 0))
					enabled = true;
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
			var choices = range.Skip(SelectedIndex);
			if (Loop)
			{
				choices = choices.Concat(range.Take(SelectedIndex));
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
