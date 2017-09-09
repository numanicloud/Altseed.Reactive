using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// オブジェクトの配置を変えないレイアウト クラス。
	/// </summary>
	public class AsIsLayout : Layouter
	{
		private List<Object2D> objects_;

		protected override IEnumerable<Object2D> ObjectsInternal => objects_;

		public IEnumerable<Object2D> Items => objects_;

		public AsIsLayout()
		{
			objects_ = new List<Object2D>();
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウトの末尾に配置します。
		/// </summary>
		/// <param name="item">配置する2Dオブジェクト。</param>
		public override void AddItem(Object2D item)
		{
			objects_.Add(item);
		}

		/// <summary>
		/// このレイアウトから全ての要素を取り除きます。
		/// </summary>
		public override void ClearItem()
		{
			objects_.Clear();
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウト上に挿入します。
		/// </summary>
		/// <param name="index">挿入する位置のインデックス。</param>
		/// <param name="item">挿入する2Dオブジェクト。</param>
		public override void InsertItem(int index, Object2D item)
		{
			objects_.Insert(index, item);
		}

		/// <summary>
		/// 2Dオブジェクトをこのレイアウトから取り除きます。
		/// </summary>
		/// <param name="item">取り除く2Dオブジェクト。</param>
		public override void RemoveItem(Object2D item)
		{
			objects_.Remove(item);
		}
	}
}
