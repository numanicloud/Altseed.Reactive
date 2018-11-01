using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal class WaitAttribute : TextAttribute
	{
		public override bool IsContinue => false;

		public override TextSetting ModifySetting(TextSetting source, string expression)
		{
			return source;
		}

		public override async Task BeforeShowText(string expression)
		{
			var time = float.Parse(expression);
			await Observable.Timer(TimeSpan.FromSeconds(time));
		}
	}
}
