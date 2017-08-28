using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI.Cursor
{
	/// <summary>
	/// 他のオブジェクトに追従するカーソルを描画するオブジェクト。
	/// </summary>
	public class Cursor : ReactiveTextureObject2D
	{
		private bool isHidden_;

		public bool IsHidden
		{
			get { return isHidden_; }
			set
			{
				if (isHidden_ == value)
				{
					return;
				}

				isHidden_ = true;

				if (value)
				{
					IsDrawn = false;
					if (Parent is IActivatableSelectionItem item)
					{
						item.Disactivate();
					}
				}
				else
				{
					IsDrawn = true;
					if (Parent is IActivatableSelectionItem item)
					{
						item.Activate();
					}
				}
			}
		}

		/// <summary>
		/// このカーソルの追従するオブジェクトを変更します。
		/// </summary>
		/// <param name="target">追従する先となるオブジェクト。</param>
		/// <remarks>カーソル オブジェクトは追従先のオブジェクトの子オブジェクトとなります。</remarks>
		public virtual void MoveTo(Object2D target)
		{
			if (!IsHidden)
			{
				if (Parent is IActivatableSelectionItem item)
				{
					item.Disactivate();
				}
				if (target is IActivatableSelectionItem item2)
				{
					item2.Activate();
				}
			}

			if (target != null)
			{
				if (IsHidden)
				{
					IsHidden = false;
				}
				if (Parent != null)
				{
					MoveCursor(target);
				}
				else
				{
					SuddenlyMoveCursor(target);
				}
			}
			else
			{
				IsHidden = false;
			}
		}

		private void MoveCursor(Object2D target)
		{
			Position = GetGlobalPosition() - target.GetGlobalPosition();
			Parent?.RemoveChild(this);
			target.AddChild(this, ChildManagementMode.Nothing, ChildTransformingMode.All);
		}

		private void SuddenlyMoveCursor(Object2D target)
		{
			Position = new Vector2DF(0, 0);
		}

		/// <summary>
		/// オーバーライドして、指定したオブジェクトへ移動するアニメーションを実装できます。
		/// </summary>
		/// <param name="obj">追従する先となるオブジェクト。</param>
		/// <remarks>追従先のオブジェクトの子オブジェクトとなるため、(0, 0)の位置が追従先のオブジェクトの位置となります。</remarks>
		protected virtual void AnimateMove(Object2D obj)
		{
			Position = new Vector2DF(0, 0);
		}
	}
}
