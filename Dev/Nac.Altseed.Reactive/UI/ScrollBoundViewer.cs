using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.Reactive.UI
{
	public class ScrollBoundViewer : TextureObject2D
	{
		private ScrollLayer scrollLayer { get; set; }
		private CameraObject2D debugCamera { get; set; }
		private GeometryObject2D outerBinding { get; set; }
		private GeometryObject2D innerBinding { get; set; }

		public ScrollBoundViewer(ScrollLayer scrollLayer)
		{
			this.scrollLayer = scrollLayer;
			debugCamera = new CameraObject2D()
			{
				Src = Helper.GetRectFromVector(
					(scrollLayer.Starting - new Vector2DF(30, 30)).To2DI(),
					(scrollLayer.Ending - scrollLayer.Starting + new Vector2DF(60, 60)).To2DI()),
				Dst = new RectI(Engine.WindowSize.X / 2, 0, Engine.WindowSize.X / 2, Engine.WindowSize.Y),
			};
			outerBinding = new GeometryObject2D()
			{
				DrawingPriority = -2,
				Color = new Color(255, 0, 0, 128),
			};
			innerBinding = new GeometryObject2D()
			{
				DrawingPriority = -1,
				Color = new Color(0, 0, 255, 128),
			};
		}

		protected override void OnUpdate()
		{
			outerBinding.Shape = new RectangleShape()
			{
				DrawingArea = Helper.GetRectFromVector(scrollLayer.Starting.To2DI(), (scrollLayer.Ending - scrollLayer.Starting).To2DI()).ToFloat(),
			};
			innerBinding.Shape = new RectangleShape()
			{
				DrawingArea = Helper.ShiftRect(scrollLayer.BindingAreaRange, scrollLayer.CameraSrc.Position.To2DF()),
			};
		}

		protected override void OnStart()
		{
			Layer.AddObject(debugCamera);
			Layer.AddObject(outerBinding);
			Layer.AddObject(innerBinding);
		}

		protected override void OnVanish()
		{
			debugCamera.Vanish();
			outerBinding.Vanish();
			innerBinding.Vanish();
		}
	}
}
