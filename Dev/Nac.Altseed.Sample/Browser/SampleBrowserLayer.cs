using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Sample.Browser;
using Nac.Altseed.UI;

namespace sample_cs
{
	class SampleBrowserLayer : Layer2D
	{
		public static readonly int Columns = 3;
		public static readonly Vector2DF ItemOffset = new Vector2DF(632 / Columns, 150);
		
		private CameraObject2D camera;
		private Subject<float> onScrollSubject_ = new Subject<float>();

		public IObservable<ISample> OnSelectionChanged => Selector.OnSelectionChanged.Select(x => x?.Choice);
		public IObservable<ISample> OnDecide => Selector.OnClick.Select(x => x?.Choice);
		public IObservable<float> OnScroll => onScrollSubject_;
		public float TotalHeight => Layouter.DrawnArea.Height;

		private TableLayouter Layouter { get; set; }
		private PointingSelector<ISample> Selector { get; set; }


		public SampleBrowserLayer(ISample[] samples)
		{
            Name = "BrowserLayer";

			Layouter = new TableLayouter()
			{
				ItemSpan = ItemOffset,
				Position = new Vector2DF(8, 8),
				LineCapacity = 3,
			};
			Selector = new PointingSelector<ISample>(Layouter);
			AddObject(Selector);

			var font = Engine.Graphics.CreateDynamicFont("", 12, new Color(255, 255, 255, 255), 1, new Color(0, 0, 0, 255));
			foreach(var sample in samples)
			{
				var item = new SampleItem(sample, font);
				Selector.AddChoice(sample, item, SampleItem.Size);
			}

            camera = new CameraObject2D()
			{
				Src = new RectI(0, 0, 640, (int)SampleBrowser.ViewerHeight),
				Dst = new RectI(0, 20, 640, (int)SampleBrowser.ViewerHeight),
			};
			AddObject(camera);
		}

		protected override void OnUpdated()
		{
            var y = camera.Src.Y - Engine.Mouse.MiddleButton.WheelRotation * 20;
			y = Math.Max(0, y);
			y = Math.Min(TotalHeight - SampleBrowser.ViewerHeight, y);
			camera.Src = new RectI(
				camera.Src.X,
				(int)y,
				camera.Src.Width,
				camera.Src.Height);
			onScrollSubject_.OnNext(y);

			Selector.MouseOffset = new Vector2DF(
				0 - SampleBrowser.Margin,
				y - SampleBrowser.HintHeight - SampleBrowser.Margin);
		}
	}
}
