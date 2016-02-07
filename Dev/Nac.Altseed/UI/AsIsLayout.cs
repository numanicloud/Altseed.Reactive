using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.UI
{
	public class AsIsLayout : Layouter
	{
		private List<Object2D> objects_;

		internal override IEnumerable<Object2D> ObjectsInternal => objects_;

		public IEnumerable<Object2D> Items => objects_;

		public AsIsLayout()
		{
			objects_ = new List<Object2D>();
		}

		public override void AddItem(Object2D obj)
		{
			objects_.Add(obj);
		}

		public override void ClearItem()
		{
			objects_.Clear();
		}

		public override void InsertItem(int index, Object2D obj)
		{
			objects_.Insert(index, obj);
		}

		public override void RemoveItem(Object2D obj)
		{
			objects_.Remove(obj);
		}
	}
}
