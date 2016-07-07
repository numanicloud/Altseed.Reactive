using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using asd;
using Nac.Altseed.Sample.Browser;

namespace sample_cs
{
	internal class SampleBrowserLayer : Layer2D
	{
		public static readonly int Columns = 3;
		public static readonly Vector2DF ItemOffset = new Vector2DF((640 - SampleBrowser.Margin)/Columns, 150);

		private readonly CameraObject2D camera;
		private readonly Subject<float> onScrollSubject_ = new Subject<float>();

		public IObservable<ISample> OnSelectionChanged => Selector.OnSelectionChanged.Select(x => x?.Choice);
		public IObservable<ISample> OnDecide => Selector.OnClick.Select(x => x?.Choice);
		public IObservable<float> OnScroll => onScrollSubject_;
		public float TotalHeight => Math.Max(Layouter.DrawnArea.Height, SampleBrowser.ViewerHeight);

		private TableLayouter Layouter { get; }
		private PointingSelector<ISample> Selector { get; }


		public SampleBrowserLayer(ISample[] samples)
		{
			Name = "BrowserLayer";

			Layouter = new TableLayouter
			{
				ItemSpan = ItemOffset,
				Position = new Vector2DF(SampleBrowser.Margin + 16, SampleBrowser.Margin),
				LineCapacity = 3
			};
			Selector = new PointingSelector<ISample>(Layouter);
			AddObject(Selector);

			var font = Engine.Graphics.CreateDynamicFont("", 12, new Color(255, 255, 255, 255), 1, new Color(0, 0, 0, 255));
			foreach (var sample in samples)
			{
				var item = new SampleItem(sample, font);
				Selector.AddChoice(sample, item, SampleItem.Size);
			}

			camera = new CameraObject2D
			{
				Src = new RectI(0, 0, 640, (int) SampleBrowser.ViewerHeight),
				Dst = new RectI(0, 20, 640, (int) SampleBrowser.ViewerHeight)
			};
			AddObject(camera);
		}

		protected override void OnUpdated()
		{
			var y = camera.Src.Y - Engine.Mouse.MiddleButton.WheelRotation*20;
			y = Math.Max(0, y);
			y = Math.Min(Math.Max(TotalHeight - SampleBrowser.ViewerHeight, 0), y);
			camera.Src = new RectI(
				camera.Src.X,
				(int) y,
				camera.Src.Width,
				camera.Src.Height);
			onScrollSubject_.OnNext(y);

			Selector.MouseOffset = new Vector2DF(0, y - SampleBrowser.HintHeight) - Layouter.Position;
		}
	}
}