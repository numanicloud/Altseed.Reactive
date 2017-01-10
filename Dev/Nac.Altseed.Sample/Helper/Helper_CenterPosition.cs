using asd;	// Engineクラスなどをフルネームで書かずに済ますように

namespace Nac.Altseed.Sample.Helper
{
	class Helper_CenterPosition : ISample
	{
		private void AddNewObject(Vector2DF position, CenterPosition centerPosition)
		{
			var obj = new TextureObject2D();
			obj.Texture = Engine.Graphics.CreateTexture2D("Data/Helper/Tree.png");
			obj.SetCenterPosition(centerPosition);
			obj.Position = position + new Vector2DF(20, 20);
			Engine.AddObject2D(obj);

			var center = new GeometryObject2D();
			var circle = new CircleShape();
			circle.OuterDiameter = 10;
			circle.Position = position + new Vector2DF(20, 20);
			center.Shape = circle;
			Engine.AddObject2D(center);
		}

		public void Run()
		{
			Engine.Initialize("Helper_CenterPosition", 640, 480, new EngineOption());

			AddNewObject(new Vector2DF(0, 0), CenterPosition.TopLeft);
			AddNewObject(new Vector2DF(225, 0), CenterPosition.TopCenter);
			AddNewObject(new Vector2DF(450, 0), CenterPosition.TopRight);
			AddNewObject(new Vector2DF(0, 225), CenterPosition.CenterLeft);
			AddNewObject(new Vector2DF(225, 225), CenterPosition.CenterCenter);
			AddNewObject(new Vector2DF(450, 225), CenterPosition.CenterRight);
			AddNewObject(new Vector2DF(0, 450), CenterPosition.BottomLeft);
			AddNewObject(new Vector2DF(225, 450), CenterPosition.BottomCenter);
			AddNewObject(new Vector2DF(450, 450), CenterPosition.BottomRight);

			// メインループ
			while (Engine.DoEvents())
			{
				Engine.Update();
				Recorder.TakeScreenShot("Helper_CenterPosition", 20);
			}

			Engine.Terminate();
		}

		public string Description => "オブジェクトの中心位置を簡単に設定するサンプルです。";
		public string Title => "オブジェクトの中心位置";
	}
}
