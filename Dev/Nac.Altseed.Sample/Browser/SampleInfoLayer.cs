using asd;

namespace sample_cs
{
	internal class SampleInfoLayer : Layer2D
	{
		private static readonly float ScrollBerWidth = 16;
		private static readonly float ScrollBarX = 640 - ScrollBerWidth - SampleBrowser.Margin;

		private TextObject2D Title { get; }
		private TextObject2D ClassName { get; }
		private TextObject2D Description { get; }
		private GeometryObject2D ScrollBar { get; }
		private float TotalHeight { get; }

		public SampleInfoLayer(float totalHeight)
		{
			var scrollBarHeight = SampleBrowser.ViewerHeight / totalHeight * SampleBrowser.ViewerHeight;
			ScrollBar = new GeometryObject2D
			{
				Shape = new RectangleShape
				{
					DrawingArea = new RectF(0, 0, ScrollBerWidth, scrollBarHeight - SampleBrowser.Margin * 2)
				},
				Color = new Color(64, 64, 64, 255),
				Position = new Vector2DF(ScrollBarX, SampleBrowser.DescriptionHeight + SampleBrowser.Margin),
				DrawingPriority = 9
			};
			TotalHeight = totalHeight;
			AddObject(ScrollBar);

			Name = "InfoLayer";

			var panel = new GeometryObject2D
			{
				Shape = new RectangleShape
				{
					DrawingArea = new RectF(0, 0, 640, SampleBrowser.DescriptionHeight)
				},
				Color = new Color(16, 16, 16, 255),
				Position = new Vector2DF(0, 480 - SampleBrowser.DescriptionHeight)
			};

			var font = Engine.Graphics.CreateDynamicFont("", 12, new Color(255, 255, 255, 255), 1, new Color(0, 0, 0, 255));
			Title = new TextObject2D
			{
				Font = font,
				Text = "",
				Color = new Color(255, 255, 0),
				Position = new Vector2DF(2, 2),
				DrawingPriority = 1
			};

			ClassName = new TextObject2D
			{
				Font = font,
				Text = "",
				Color = new Color(128, 255, 225),
				Position = new Vector2DF(2, 2),
				DrawingPriority = 1
			};

			Description = new TextObject2D
			{
				Font = font,
				Text = "",
				Color = new Color(255, 255, 255),
				Position = new Vector2DF(6, 22),
				DrawingPriority = 1
			};

			panel.AddChild(Title, ChildManagementMode.Nothing, ChildTransformingMode.Position);
			panel.AddChild(Description, ChildManagementMode.Nothing, ChildTransformingMode.Position);
			panel.AddChild(ClassName, ChildManagementMode.RegistrationToLayer, ChildTransformingMode.Position);

			AddObject(panel);
			AddObject(Title);
			AddObject(Description);
		}

		public void Show(ISample sample)
		{
			if (sample == null)
			{
				Title.Text = "";
				Description.Text = "";
				ClassName.Text = "";
			}
			else
			{
				Title.Text = sample.Title;
				ClassName.Text = "(" + sample.GetType().Name + ")";
				Description.Text = sample.Description;
				ClassName.Position = new Vector2DF(Title.Font.CalcTextureSize(Title.Text, WritingDirection.Horizontal).X + 8, 2);
			}
		}

		public void MoveScrollBar(float pos)
		{
			var yOffset = pos / TotalHeight * SampleBrowser.ViewerHeight;
			ScrollBar.Position = new Vector2DF(640 - ScrollBerWidth - SampleBrowser.Margin, 20 + SampleBrowser.Margin + yOffset);
		}
	}
}