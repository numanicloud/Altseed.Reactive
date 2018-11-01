using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal class TalkSpeedAttribute : TextAttribute
	{
		public override bool IsContinue => true;

		public override TextSetting ModifySetting(TextSetting source, string expression)
		{
			var speed = float.Parse(expression);
			return source.WithSpeed(speed);
		}
	}
}
