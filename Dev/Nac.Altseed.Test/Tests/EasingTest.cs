using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed;

namespace Nac.Altseed.Test
{
    class EasingTest : AltseedTest
    {
        protected override void OnStart()
        {
            AddObject(new Vector2DF(50, 50), EasingStart.Start, EasingEnd.End);
            AddObject(new Vector2DF(50, 150), EasingStart.StartRapidly2, EasingEnd.EndSlowly2);
            AddObject(new Vector2DF(50, 250), EasingStart.StartRapidly2, EasingEnd.EndRapidly2);
        }

        private void AddObject(Vector2DF position, EasingStart start, EasingEnd end)
        {
            var heart = Engine.Graphics.CreateTexture2D("Heart.png");
            var obj = new TextureObject2D()
            {
                Texture = heart,
                Position = position,
            };
            obj.SetCenterPosition(CenterPosition.CenterCenter);
            Engine.AddObject2D(obj);

            obj.SetEasingX(position.X + 500, start, end, 120);
        }
    }
}
