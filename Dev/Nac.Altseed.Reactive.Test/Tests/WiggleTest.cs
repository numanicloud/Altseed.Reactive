using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive;

namespace Nac.Altseed.Reactive.Test
{
    class WiggleTest : AltseedTest
    {
        private Vector2DF center;
        private TextureObject2D obj;
        private Cancelable cancel;
        
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
                cancel = ReactiveAction.SetShortWiggle(obj, center, new Vector2DF(10, 10), new Vector2DF(10, 7), 3);
            }
            if(cancel != null && Engine.Keyboard.GetKeyState(Keys.X) == KeyState.Push)
            {
                cancel.Dispose();
            }
        }
    }
}
