using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed;

namespace Nac.Altseed.Test
{
    class WiggleTest : AltseedTest
    {
        private Vector2DF center;
        private TextureObject2D obj;
        private IDisposable cancel;
        
        protected override void OnStart()
        {
            center = new Vector2DF(320, 240);
            obj = new TextureObject2D()
            {
                Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
                Position = center,
            };
            Engine.AddObject2D(obj);

            var obj2 = new TextureObject2D()
            {
                Texture = Engine.Graphics.CreateTexture2D("Heart.png"),
                Position = center,
                DrawingPriority = -1,
            };
            Engine.AddObject2D(obj2);
        }

        protected override void OnUpdate()
        {
            if(Engine.Keyboard.GetKeyState(Keys.Z) == KeyState.Push)
            {
                cancel?.Dispose();
				cancel = UpdateManager.Instance.FrameUpdate
					.ShortWiggle(center, new Vector2DF(10, 10), new Vector2DF(10, 7), 180)
					.Subscribe(p => obj.Position = p);
            }
            if(cancel != null && Engine.Keyboard.GetKeyState(Keys.X) == KeyState.Push)
            {
                cancel.Dispose();
            }
        }
    }
}
