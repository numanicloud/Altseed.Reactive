using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.Test
{
    class EasingTest : AltseedTest
    {
        protected override void OnStart()
        {
            AddObject(new Vector2DF(50, 50), EasingStart.Start, EasingEnd.End);
            AddObject(new Vector2DF(150, 50), EasingStart.StartRapidly2, EasingEnd.EndSlowly2);
            AddObject(new Vector2DF(250, 50), EasingStart.StartRapidly2, EasingEnd.EndRapidly2);
			AddObject(new Vector2DF(350, 50), EasingStart.StartRapidly2, EasingEnd.EndSlowly3);
		}

        private void AddObject(Vector2DF position, EasingStart start, EasingEnd end)
        {
            var heart = Engine.Graphics.CreateTexture2D("Heart.png");
            var obj = new ReactiveTextureObject2D()
            {
                Texture = heart,
                Position = position,
            };
            obj.SetCenterPosition(CenterPosition.CenterCenter);
            Engine.AddObject2D(obj);

			obj.OnUpdateEvent.DoEasingY(obj, position.Y + 400, start, end, 120)
				.Subscribe();
        }
    }
}
