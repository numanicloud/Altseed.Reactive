using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;
using Nac.Altseed.UI;

namespace Nac.Altseed.Sample.Browser
{
	public class PointingSelector<TChoice> : ReactiveTextureObject2D where TChoice : class
	{
		public class Item
		{
			public Item(TChoice choice, Object2D o, Shape shape)
			{
				Choice = choice;
				Object = o;
				Shape = shape;
			}

			public TChoice Choice { get; private set; }
			public Object2D Object { get; private set; }
			public Shape Shape { get; private set; }
		}

		private List<Item> items_;
		private Layouter layouter_;
		private Subject<Item> onSelectionChangedSubject_ = new Subject<Item>();
		private Subject<Item> onClickSubject_ = new Subject<Item>();
		private CircleShape mouseShape_;

		public IObservable<Item> OnSelectionChanged => onSelectionChangedSubject_;
		public IObservable<Item> OnClick => onClickSubject_;
		public Vector2DF MouseOffset { get; set; }
		public Item SelectedItem { get; private set; }

		public PointingSelector(Layouter layouter)
		{
			this.layouter_ = layouter;
			AddDrawnChild(layouter, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Disposal, ChildTransformingMode.Position, ChildDrawingMode.DrawingPriority);

			items_ = new List<Item>();
			mouseShape_ = new CircleShape()
			{
				OuterDiameter = 2,
				NumberOfCorners = 12,
			};
			SelectedItem = null;
		}

		public void AddChoice(TChoice choice, Object2D obj, Vector2DF size)
		{
			layouter_.AddItem(obj);
			var rect = new RectangleShape()
			{
				DrawingArea = new RectF(obj.Position, size)
			};
			var item = new Item(choice, obj, rect);
			items_.Add(item);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			mouseShape_.Position = Engine.Mouse.Position + MouseOffset;
			
			Item next = null;
			foreach (var item in items_)
			{
				if (next == null && item.Shape.GetIsCollidedWith(mouseShape_))
				{
					if (SelectedItem != item)
					{
						(item.Object as IActivatableSelectionItem)?.Activate();
					}
					next = item;
				}
				else if(SelectedItem == item)
				{
					(item.Object as IActivatableSelectionItem)?.Disactivate();
				}
			}
			if (next != SelectedItem)
			{
				onSelectionChangedSubject_.OnNext(next);
			}
			SelectedItem = next;

			if(Engine.Mouse.LeftButton.ButtonState == MouseButtonState.Push)
			{
				onClickSubject_.OnNext(SelectedItem);
			}
		}
	}
}
