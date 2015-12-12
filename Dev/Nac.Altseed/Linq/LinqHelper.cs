using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Linq
{
    public static class LinqHelper
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
	}
}
