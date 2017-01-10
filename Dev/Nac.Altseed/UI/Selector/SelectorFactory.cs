using Nac.Altseed.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.UI.Selector
{
	public static class SelectorFactory
	{
		private enum SelectorControl
		{
			Next, Previous, Decide, Cancel
		}

		/// <summary>
		/// 構築済みの選択UIオブジェクトを生成します。
		/// </summary>
		/// <typeparam name="TChoice">選択肢の型。</typeparam>
		/// <param name="itemSpan">選択肢オブジェクトどうしの間隔。</param>
		/// <returns></returns>
		public static ISelector<TChoice> CreateSimpleSelector<TChoice>(Vector2DF itemSpan)
		{
			var controller = new KeyboardController<SelectorControl>();
			var layout = new LinearPanel();
			var selector = new Selector<TChoice, SelectorControl>(controller, layout);
			selector.IsActive = true;
			selector.BindKey(SelectorControl.Next, SelectorControl.Previous, SelectorControl.Decide, SelectorControl.Cancel);

			return selector;
		}
	}
}
