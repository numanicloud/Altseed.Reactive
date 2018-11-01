using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.Attributes
{
	internal class AttributeFacade
	{
		private static AttributeFacade instance_;

		private Dictionary<string, TextAttribute> Attributes { get; set; }

		public static AttributeFacade Instance
		{
			get { return instance_ ?? (instance_ = new AttributeFacade()); }
		}

		private AttributeFacade()
		{
			Attributes = new Dictionary<string, TextAttribute>
			{
				["\\c"] = new ColorAttribute(),
				["\\n"] = new NewLineAttribute(),
				["\\p"] = new NewPageAttribute(),
				["\\s"] = new TalkSpeedAttribute(),
				["\\w"] = new WaitAttribute(),
			};
		}

		public TextAttribute GetAttribute(string attributeToken)
		{
			return Attributes.ContainsKey(attributeToken) ? Attributes[attributeToken]
				: null;
		}
	}
}
