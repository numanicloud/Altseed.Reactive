using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed.UI
{
	public class ScrollBoundViewer : TextureObject2D
	{
		private ScrollLayer scrollLayer { get; set; }
		private CameraObject2D debugCamera { get; set; }
		private GeometryObject2D outerBinding { get; set; }
		private GeometryObject2D innerBinding { get; set; }
		private GeometryObject2D cameraBinding { get; set; }
        private GeometryObject2D seeingArea { get; set; }

		public ScrollBoundViewer(ScrollLayer scrollLayer)
		{
			this.scrollLayer = scrollLayer;
			debugCamera = new CameraObject2D()
			{
				Src = GeometoryHelper.GetRectFromVector(
					(scrollLayer.Starting - new Vector2DF(30, 30)).To2DI(),
					(scrollLayer.Ending - scrollLayer.Starting + new Vector2DF(60, 60)).To2DI()),
				Dst = new RectI(Engine.WindowSize.X / 2, 0, Engine.WindowSize.X / 2, Engine.WindowSize.Y),
			};
			outerBinding = new GeometryObject2D()
			{
				DrawingPriority = -3,
				Color = new Color(255, 0, 0, 128),
			};
			innerBinding = new GeometryObject2D()
			{
				DrawingPriority = -1,
				Color = new Color(0, 0, 255, 128),
			};
			cameraBinding = new GeometryObject2D()
			{
				DrawingPriority = -2,
				Color = new Color(0, 255, 0, 128),
			};
            seeingArea = new GeometryObject2D()
            {
                DrawingPriority = -2,
                Color = new Color(255, 255, 0, 100),
            };
			AddChild(debugCamera, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Vanishment, ChildTransformingMode.Nothing);
			AddChild(outerBinding, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Vanishment, ChildTransformingMode.Nothing);
			AddChild(innerBinding, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Vanishment, ChildTransformingMode.Nothing);
			AddChild(cameraBinding, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Vanishment, ChildTransformingMode.Nothing);
			AddChild(seeingArea, ChildManagementMode.RegistrationToLayer | ChildManagementMode.Vanishment, ChildTransformingMode.Nothing);
        }

		protected override void OnUpdate()
		{
			outerBinding.Shape = new RectangleShape()
			{
				DrawingArea = GeometoryHelper.GetRectFromVector(scrollLayer.Starting.To2DI(), (scrollLayer.Ending - scrollLayer.Starting).To2DI()).ToFloat(),
			};
			innerBinding.Shape = new RectangleShape()
			{
				DrawingArea = scrollLayer.BindingAreaRange.Shift(scrollLayer.CameraSrc.Position.To2DF()),
			};
			cameraBinding.Shape = new RectangleShape()
			{
				DrawingArea = scrollLayer.CameraSrc.ToFloat(),
			};
            seeingArea.Shape = new RectangleShape()
            {
                DrawingArea = scrollLayer.SeeingArea,
            };
		}
	}
}
