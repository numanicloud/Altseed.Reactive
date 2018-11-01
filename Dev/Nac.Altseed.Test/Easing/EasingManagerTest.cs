using asd;
using Nac.Altseed.Easings;
using Nac.Altseed.Easings.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Test.Easing
{
	class EasingManagerTest : AltseedTest
	{
		EasingManagerComponent manager;

		protected override void OnStart()
		{
			var obj = new asd.GeometryObject2D()
			{
				Shape = new asd.CircleShape()
				{
					OuterDiameter = 25,
					NumberOfCorners = 36
				},
				Position = new asd.Vector2DF(100, 30)
			};
			Engine.AddObject2D(obj);

			var easing = new CubicEasing(CubicEasing.Speed.Rapidly2, CubicEasing.Speed.Slowly3);
			manager = new EasingManagerComponent(100)
			{
				EasingSettings =
				{
					["Right"] = new EasingManagerComponent.Setting(easing, 40, 500),
					["Left"] = new EasingManagerComponent.Setting(easing, 90, 100),
					["Top"] = new EasingManagerComponent.Setting(easing, 20, 320),
				}
			};
			obj.AddComponent(manager, "Easing");

			manager.NewValue.Subscribe(x => obj.Position = obj.Position.WithX(x));
		}

		protected override void OnUpdate()
		{
			if (Engine.Keyboard.GetKeyState(Keys.Left) == ButtonState.Push)
			{
				manager.Ease("Left");
			}
			else if(Engine.Keyboard.GetKeyState(Keys.Right) == ButtonState.Push)
			{
				manager.Ease("Right");
			}
			else if (Engine.Keyboard.GetKeyState(Keys.Up) == ButtonState.Push)
			{
				manager.Ease("Top");
			}
		}
	}
}
