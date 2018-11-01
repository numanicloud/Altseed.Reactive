using Altseed.Reactive.Input;
using Altseed.Reactive.Ui.TextWindow;
using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altseed.Reactive.Test.Ui
{
	class MarkupTextViewerTest : AltseedTest
	{
		MarkupTextViewer textViewer;
		UpdatableSynchronizationContext sync;

		protected override void OnStart()
		{
			textViewer = new MarkupTextViewer(null);
			textViewer.DefaultSetting = new Reactive.Ui.TextSetting()
			{
				TalkSpeed = 8,
				TextColor = new Color(255, 255, 255),
			};

			var controller = new KeyboardController<int>();
			controller.BindKey(Keys.Z, 0);
			textViewer.BindControlToRead(controller, 0);

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
			await Task.Delay(3000);
			await textViewer.ShowStyledText(@"敵の\c[ff0000]弱点属性\c[ffffff]で攻撃すると、ダメージが増え痛みが減る。
\w[0.3]敵の\c[0066ff]耐性属性\c[ffffff]で攻撃すると、ダメージが減り痛みが増える。
\w[0.3]目的に応じて使い分けよう。
\k\p時々出てくるヒントは、護身術教室へ行けば見直せる。
\w[0.3]忘れてしまったら行ってみよう。");
		}
	}
}
