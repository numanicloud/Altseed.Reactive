using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nac.Altseed.UI.ChoiceList;

namespace Nac.Altseed.UnitTest
{
	[TestClass]
	public class LinearChoiceListTest
	{
		[TestMethod]
		public void 選択肢を追加できる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");

			subject.Size.Is(1);
			subject.Choices.ElementAt(0).Is("Hoge");
		}

		[TestMethod]
		public void 初期状態の選択肢はマイナス1()
		{
			var subject = new LinearChoiceList<string>();
			subject.SelectedIndex.Is(-1);
		}

		[TestMethod]
		public void 選択肢を1つ追加すると選択される()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.SelectedChoice.Is("Hoge");
		}

		[TestMethod]
		public void 選択肢を2つ登録して次の選択肢へ移動できる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");

			subject.SelectNextIndex().IsTrue();

			subject.SelectedChoice.Is("Fuga");
		}

		[TestMethod]
		public void 選択肢をループして移動できる()
		{
			var subject = new LinearChoiceList<string>()
			{
				Loop = true
			};
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");

			subject.SelectPreviousIndex().IsTrue();

			subject.SelectedChoice.Is("Fuga");
		}

		[TestMethod]
		public void 前方の選択肢を削除すると選択位置をずらす()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");
			subject.SelectedIndex = 2;

			subject.RemoveChoice("Hoge").IsTrue();

			subject.SelectedIndex.Is(1);
		}

		[TestMethod]
		public void 後方の選択肢を削除しても選択位置をずらさない()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");
			subject.SelectedIndex = 0;

			subject.RemoveChoice("Fuga").IsTrue();

			subject.SelectedIndex.Is(0);
		}

		[TestMethod]
		public void 選択中の選択肢を削除すると選択位置をずらす()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");
			subject.SelectedIndex = 2;

			subject.RemoveChoice("Piyo").IsTrue();

			subject.SelectedIndex.Is(1);
		}

		[TestMethod]
		public void 選択肢を強制的に変更できる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");

			subject.SetSelectedChoice("Piyo").IsTrue();

			subject.SelectedChoice.Is("Piyo");
		}

		[TestMethod]
		public void ループしない設定の時はループしない()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");

			subject.SelectPreviousIndex().IsFalse();

			subject.SelectedChoice.Is("Hoge");
		}

		[TestMethod]
		public void 選択肢を削除できる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.AddChoice("Piyo");

			subject.RemoveChoice("Fuga");

			subject.Choices.Count().Is(2);
			subject.Choices.ElementAt(1).Is("Piyo");
		}

		[TestMethod]
		public void 選択肢を挿入できる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");

			subject.InsertChoice(1, "Piyo");

			subject.Size.Is(3);
			subject.Choices.ElementAt(1).Is("Piyo");
		}

		[TestMethod]
		public void 前方に選択肢を挿入すると選択位置をずらす()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.SelectedIndex = 1;

			subject.InsertChoice(1, "Piyo");

			subject.SelectedIndex.Is(2);
		}

		[TestMethod]
		public void 後方に選択肢を挿入しても選択位置をずらさない()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");
			subject.SelectedIndex = 0;

			subject.InsertChoice(1, "Piyo");

			subject.SelectedIndex.Is(0);
		}

		[TestMethod]
		public void 選択肢をクリアできる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");

			subject.ClearChoice();

			subject.Size.Is(0);
		}

		[TestMethod]
		public void 選択肢をクリアすると選択位置はマイナス1になる()
		{
			var subject = new LinearChoiceList<string>();
			subject.AddChoice("Hoge");
			subject.AddChoice("Fuga");

			subject.ClearChoice();

			subject.SelectedIndex.Is(LinearChoiceList<string>.Disabled);
		}
	}
}
