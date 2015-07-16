using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Nac.Altseed
{
    /// <summary>
    /// 描画原点を指定するための列挙体。左上から右下まで９箇所を指定できます。
    /// </summary>
	public enum CenterPosition
	{
		TopLeft, TopCenter, TopRight,
		CenterLeft, CenterCenter, CenterRight,
		BottomLeft, BottomCenter, BottomRight,
	}

	public static class CenterPositionHelper
	{
        /// <summary>
        /// TextureObject2Dの描画原点を、指定した画像上の位置に変更します。
        /// </summary>
        /// <param name="obj">描画原点を指定する対象のオブジェクト。</param>
        /// <param name="centerPosition">画像上のどの位置を描画原点に指定するか。</param>
		public static void SetCenterPosition(this TextureObject2D obj, CenterPosition centerPosition)
		{
			obj.CenterPosition = GetPosition(obj.Texture.Size, centerPosition);
		}

        /// <summary>
        /// TextObject2Dの描画原点を、指定した文字列上の位置に変更します。
        /// </summary>
        /// <param name="obj">描画原点を指定する対象のオブジェクト。</param>
        /// <param name="centerPosition">文字列上のどの位置を描画原点に指定するか。</param>
		public static void SetCenterPosition(this TextObject2D obj, CenterPosition centerPosition)
		{
			var size = obj.Font.CalcTextureSize(obj.Text, obj.WritingDirection);
			obj.CenterPosition = GetPosition(size, centerPosition);
		}

		private static Vector2DF GetPosition(Vector2DI size, CenterPosition centerPosition)
		{
			switch(centerPosition)
			{
			case CenterPosition.TopLeft: return new Vector2DF(0, 0);
			case CenterPosition.TopCenter: return new Vector2DF(size.X / 2, 0);
			case CenterPosition.TopRight: return new Vector2DF(size.X, 0);
			case CenterPosition.CenterLeft: return new Vector2DF(0, size.Y / 2);
			case CenterPosition.CenterCenter: return new Vector2DF(size.X / 2, size.Y / 2);
			case CenterPosition.CenterRight: return new Vector2DF(size.X, size.Y / 2);
			case CenterPosition.BottomLeft: return new Vector2DF(0, size.Y);
			case CenterPosition.BottomCenter: return new Vector2DF(size.X / 2, size.Y);
			case CenterPosition.BottomRight: return new Vector2DF(size.X, size.Y);
			default: throw new Exception();
			}
		}
	}
}
