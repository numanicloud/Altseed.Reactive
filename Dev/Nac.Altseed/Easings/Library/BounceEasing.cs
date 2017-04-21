using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Altseed.Easings.Library
{
	public class BounceEasing : IEasing
	{
		private float speed;
		private float elasticity;

		public BounceEasing(float speed, float elasticity)
		{
			if (elasticity <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(elasticity), "弾性係数は0より大きい必要があります。");
			}
			this.speed = speed;
			this.elasticity = elasticity;
		}

		public float GetNormalValue(float time)
		{
			return 1 - (float)Math.Abs(Math.Cos(time * speed)) * (float)Math.Pow(1 - time, 1 / elasticity);
		}
	}
}
