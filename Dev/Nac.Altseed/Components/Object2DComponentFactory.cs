using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Components
{
	static class Object2DComponentFactory
	{
		public static EasingComponent2D<Object2D> CreatePositionEasing(Vector2DF goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent2D<Object2D>(
				o => o.Position,
				(o, v) => o.Position = v,
				goal,
				start,
				end,
				count,
				callback);
		}

		public static EasingComponent<Object2D> CreateXEasing(float goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent<Object2D>(
				o => o.Position.X,
				(o, v) => o.Position = new Vector2DF(v, o.Position.Y),
				goal,
				start,
				end,
				count,
				callback);
		}

		public static EasingComponent<Object2D> CreateYEasing(float goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent<Object2D>(
				o => o.Position.Y,
				(o, v) => o.Position = new Vector2DF(o.Position.X, v),
				goal,
				start,
				end,
				count,
				callback);
		}

		public static EasingComponent<TextureObject2D> CreateAlphaEasing(byte goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent<TextureObject2D>(
				o => o.Color.A,
				(o, v) => o.Color = new Color(o.Color.R, o.Color.G, o.Color.B, (byte)v),
				goal,
				start,
				end,
				count,
				callback);
		}

		public static EasingComponent2D<CameraObject2D> CreateSrcEasing(Vector2DF goal, EasingStart start, EasingEnd end, int count, Action callback = null)
		{
			return new EasingComponent2D<CameraObject2D>(
				o => o.Src.Position.To2DF(),
				(o, v) =>
				{
					var p = v.To2DI();
					o.Src = new RectI(p.X, p.Y, o.Src.Width, o.Src.Height);
				},
				goal,
				start,
				end,
				count,
				callback);
		}
	}
}
