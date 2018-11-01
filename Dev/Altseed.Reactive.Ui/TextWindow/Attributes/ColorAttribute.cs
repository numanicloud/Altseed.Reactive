using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal class ColorAttribute : TextAttribute
	{
		public override bool IsContinue => true;
		public override TextSetting ModifySetting(TextSetting source, string expression)
		{
			var red = byte.Parse(expression.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var green = byte.Parse(expression.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var blue = byte.Parse(expression.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			return source.WithColor(new Color(red, green, blue));
		}
	}
}
