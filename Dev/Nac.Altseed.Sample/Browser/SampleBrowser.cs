using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace sample_cs
{
	class SampleBrowser
	{
        public static readonly float Margin = 8;
		public static readonly float HintHeight = 20;
		public static readonly float DescriptionHeight = 65;
		public static readonly float ViewerHeight = 480 - HintHeight - DescriptionHeight;

		private ISample[] samples;

		public SampleBrowser(ISample[] samples)
		{
			this.samples = samples;
		}

		public void Run()
		{
			while(true)
			{
				ISample selected = null;

				Engine.Initialize("サンプルブラウザ", 640, 480, new EngineOption(){ GraphicsDevice = GraphicsDeviceType.DirectX11 });

				var scene = new Scene();
				var layer = new SampleBrowserLayer(samples);
                var infoLayer = new SampleInfoLayer(layer.TotalHeight)
                {
	                DrawingPriority = 2
                };

				layer.OnScroll.Subscribe(y => infoLayer.MoveScrollBar(y));
				layer.OnSelectionChanged.Subscribe(s => infoLayer.Show(s));
				layer.OnDecide.Subscribe(s => selected = s);

				Engine.ChangeScene(scene);
				scene.AddLayer(layer);
                scene.AddLayer(infoLayer);

				var hintLayer = new Layer2D();
				hintLayer.AddObject(new TextureObject2D()
				{
					Texture = Engine.Graphics.CreateTexture2D("Data/Browser/Hint.png")
				});

				scene.AddLayer(hintLayer);

				while(Engine.DoEvents() && selected == null)
				{
					Engine.Update();
				}

				Engine.Terminate();

				if(selected == null)
				{
					break;
				}
				selected.Run();
			}
		}
	}
}
