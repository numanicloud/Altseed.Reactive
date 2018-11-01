using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal class NewPageAttribute : TextAttribute
	{
		public override bool IsContinue => false;

		public override TextSetting ModifySetting(TextSetting source, string expression)
		{
			return source.WithNewPage();
		}
	}
}
