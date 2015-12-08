using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive
{
    public static class Helper
    {
		/// <summary>
		/// コレクションの要素から指定した条件を最初に満たすオブジェクトを検索し、そのインデックスを返します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">検索対象となるコレクション。</param>
		/// <param name="predicate">検索する要素の条件を定義するデリゲート。</param>
		/// <returns></returns>
		public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			int index = 0;
			foreach(var item in source)
			{
				if(predicate(item))
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		public static RectF ToFloat(this RectI rect)
		{
			return new RectF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectI ToInt(this RectF rect)
		{
			return new RectI((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static RectI WithX(this RectI rect, int x)
		{
			return new RectI(x, rect.Y, rect.Width, rect.Height);
		}

		public static RectI WithY(this RectI rect, int y)
		{
			return new RectI(rect.X, y, rect.Width, rect.Height);
		}

		public static RectI GetRectFromVector(Vector2DI position, Vector2DI size)
		{
			return new RectI(position.X, position.Y, size.X, size.Y);
		}

		public static RectF GetRectFromVector(Vector2DF position, Vector2DF size)
		{
			return new RectF(position.X, position.Y, size.X, size.Y);
		}

		public static RectF ShiftRect(RectF source, Vector2DF shift)
		{
			return new RectF(source.X + shift.X, source.Y + shift.Y, source.Width, source.Height);
		}

		public static RectF WithPosition(this RectF source, Vector2DF position)
		{
			return new RectF(position.X, position.Y, source.Width, source.Height);
		}

		public static string ToString(RectF rect)
		{
			return $"({rect.X}, {rect.Y}, {rect.Width}, {rect.Height})";
		}
	}
}
