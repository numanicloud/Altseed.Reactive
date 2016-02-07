using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	interface IScrollingSelector
	{
		TextureObject2D Cursor { get; }
		ReactiveLayer2D ScrollLayer { get; }
		Vector2DF Position { get; set; }
		Orientation Orientation { get; set; }
		float LineSpan { get; set; }
		float LineWidth { get; set; }
		int BoundLines { get; set; }
		int ExtraLinesOnStarting { get; set; }
		int ExtraLinesOnEnding { get; set; }
		Vector2DF LayoutStarting { get; set; }

		void SetEasingScrollUp(EasingStart start, EasingEnd end, int time);
		void SetDebugCameraUp();
	}
}
