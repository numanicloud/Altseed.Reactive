using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
	public static class Helper
	{
		public static Vector2DF ToFloat(this Vector2DI v)
		{
			return new Vector2DF(v.X, v.Y);
		}

		public static RectF ToFloat(this RectI r)
		{
			var pos = r.Position;
			var size = r.Size;
			return new RectF(pos.X, pos.Y, size.X, size.Y);
		}

		public static void AddComponentWithDummy(this Layer2D layer, Object2DComponent component, string key)
		{
			var dummy = new TextureObject2D();
			dummy.IsDrawn = false;
			layer.AddObject(dummy);
			dummy.AddComponent(component, key);
		}
	}
}
