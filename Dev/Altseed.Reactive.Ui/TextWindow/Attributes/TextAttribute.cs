using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal abstract class TextAttribute
	{
		public abstract bool IsContinue { get; }
		public abstract TextSetting ModifySetting(TextSetting source, string expression);
		public virtual Task BeforeShowText(string expression) => Task.FromResult(0);
	}
}
