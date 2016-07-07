using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Sample.Browser
{
	public class TableLayouter : Layouter
	{
		private readonly List<Object2D> items_;

		protected override IEnumerable<Object2D> ObjectsInternal => items_;

		public Vector2DF ItemSpan { get; set; }
		public Orientation Orientation { get; set; }
		public int LineCapacity { get; set; } = 3;

		public RectF DrawnArea
		{
			get
			{
				var size = new Vector2DF(LineCapacity, items_.Count / LineCapacity);
				if (Orientation == Orientation.Vertical)
				{
					size = size.ExchangeXY();
				}
				if (items_.Count % LineCapacity != 0)
				{
					size += new Vector2DF(0, 1);
				}
				size *= ItemSpan;
				return new RectF(Position, size);
			}
		}

		public TableLayouter()
		{
			items_ = new List<Object2D>();
		}

		public override void AddItem(Object2D item)
		{
			item.Position = GetPosition(items_.Count);
			AddChild(item, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal, ChildTransformingMode.Position);
			items_.Add(item);
		}

		public override void InsertItem(int index, Object2D item)
		{
			item.Position = GetPosition(items_.Count);
			AddChild(item, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal, ChildTransformingMode.Position);
			items_.Insert(index, item);
		}

		public override void RemoveItem(Object2D item)
		{
			RemoveChild(item);
			items_.Remove(item);
		}

		public override void ClearItem()
		{
			foreach (var item in items_)
			{
				RemoveChild(item);
			}
			items_.Clear();
		}

		private Vector2DF GetPosition(int index)
		{
			var x = index % LineCapacity;
			var y = index / LineCapacity;
			var offset = new Vector2DF(x, y);
			if (Orientation == Orientation.Vertical)
			{
				offset = offset.ExchangeXY();
			}
			return offset * ItemSpan;
		}
	}
}
