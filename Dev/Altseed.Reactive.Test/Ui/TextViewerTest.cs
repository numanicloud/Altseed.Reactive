using Altseed.Reactive.Ui;
using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Ui
{
	class TextViewerTest : AltseedTest
	{
		TextViewer textViewer;
		UpdatableSynchronizationContext sync;

		protected override void OnStart()
		{
			textViewer = new TextViewer(null);
			Engine.AddObject2D(textViewer);

			sync = new UpdatableSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(sync);

			var task = ShowTexts();
		}

		protected override void OnUpdate()
		{
			sync.Update();
		}

		private async Task ShowTexts()
		{
			await textViewer.ShowTextAsync("普通の文字列", new TextSetting()
			{
				TextColor = new Color(255, 255, 255),
				NewLine = false,
				NewPage = false,
				TalkSpeed = 4
			});
			await textViewer.ShowTextAsync("改行＋すばやく", new TextSetting()
			{
				TextColor = new Color(255, 255, 255),
				NewLine = true,
				NewPage = false,
				TalkSpeed = 6,
			});
			await textViewer.ShowTextAsync("ゆっくりと", new TextSetting()
			{
				TextColor = new Color(255, 255, 255),
				NewLine = false,
				NewPage = false,
				TalkSpeed = 2,
			});
			await textViewer.ShowTextAsync("改ページ", new TextSetting()
			{
				TextColor = new Color(255, 255, 255),
				NewLine = false,
				NewPage = true,
				TalkSpeed = 4,
			});
			await textViewer.ShowTextAsync("色付き", new TextSetting()
			{
				TextColor = new Color(255, 64, 64),
				NewLine = false,
				NewPage = false,
				TalkSpeed = 4,
			});

			var plain = new TextSetting()
			{
				TextColor = new Color(255, 255, 255),
				NewLine = false,
				NewPage = false,
				TalkSpeed = 4,
			};
			var newLine = plain.WithNewLine();
			var newPage = plain.WithNewPage();
			await textViewer.ShowTextAsync("敵の", newPage);
			await textViewer.ShowTextAsync("弱点属性", plain.WithColor(new Color(255, 32, 32)));
			await textViewer.ShowTextAsync("で攻撃すると、ダメージが増えて痛みが減る。", plain);
			await textViewer.ShowTextAsync("敵の", newLine);
			await textViewer.ShowTextAsync("耐性属性", plain.WithColor(new Color(32, 128, 255)));
			await textViewer.ShowTextAsync("で攻撃すると、ダメージが減って痛みが増える。", plain);
			await textViewer.ShowTextAsync("目的に応じて使い分けよう。", newLine);
		}
	}
}
