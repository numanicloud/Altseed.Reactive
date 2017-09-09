using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Altseed.Reactive
{
	public static class GeometoryHelper
	{
		#region Vector2DF
		public static Vector2DF ExchangeXY(this Vector2DF vector)
		{
			return new Vector2DF(vector.Y, vector.X);
		}

		public static Vector2DF WithY(this Vector2DF vector, float y)
		{
			return new Vector2DF(vector.X, y);
		}

		public static Vector2DF WithX(this Vector2DF vector, float x)
		{
			return new Vector2DF(x, vector.Y);
		}
		#endregion

		#region RectI
		public static RectI WithX(this RectI rect, int x)
		{
			return new RectI(x, rect.Y, rect.Width, rect.Height);
		}

		public static RectI WithY(this RectI rect, int y)
		{
			return new RectI(rect.X, y, rect.Width, rect.Height);
		}

		public static RectI Shift(this RectI source, Vector2DI shift)
		{
			return new RectI(source.X + shift.X, source.Y + shift.Y, source.Width, source.Height);
		}

		public static RectI WithPosition(this RectI source, Vector2DI position)
		{
			return new RectI(position.X, position.Y, source.Width, source.Height);
		}

		public static string ToString(this RectI rect)
		{
			return $"({rect.X}, {rect.Y}, {rect.Width}, {rect.Height})";
		}
		#endregion

		#region RectF
		public static RectF WithX(this RectF rect, float x)
		{
			return new RectF(x, rect.Y, rect.Width, rect.Height);
		}

		public static RectF WithY(this RectF rect, int y)
		{
			return new RectF(rect.X, y, rect.Width, rect.Height);
		}

		public static RectF Shift(this RectF source, Vector2DF shift)
		{
			return new RectF(source.X + shift.X, source.Y + shift.Y, source.Width, source.Height);
		}

		public static RectF WithPosition(this RectF source, Vector2DF position)
		{
			return new RectF(position.X, position.Y, source.Width, source.Height);
		}

		public static string ToString(this RectF rect)
		{
			return $"({rect.X}, {rect.Y}, {rect.Width}, {rect.Height})";
		}
		#endregion

		public static Color WithAlpha(this Color source, byte alpha)
		{
			return new Color(source.R, source.G, source.B, alpha);
		}
	}
}
