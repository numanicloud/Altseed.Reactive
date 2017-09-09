using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;

namespace Altseed.Reactive.Ui.Cursor
{
	/// <summary>
	/// オブジェクトが他のオブジェクトに追従するようにするコンポーネント。
	/// </summary>
	public class CursorComponent : asd.Object2DComponent
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
					Owner.IsDrawn = false;
					if (Owner.Parent is IActivatableSelectionItem item)
					{
						item.Disactivate();
					}
				}
				else
				{
					Owner.IsDrawn = true;
					if (Owner.Parent is IActivatableSelectionItem item)
					{
						item.Activate();
					}
				}
			}
		}

		public Vector2DF Offset;

		/// <summary>
		/// このカーソルの追従するオブジェクトを変更します。
		/// </summary>
		/// <param name="target">追従する先となるオブジェクト。</param>
		/// <remarks>カーソル オブジェクトは追従先のオブジェクトの子オブジェクトとなります。</remarks>
		public virtual void MoveTo(Object2D target)
		{
			if (!IsHidden)
			{
				if (Owner.Parent is IActivatableSelectionItem item)
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
				if (Owner.Parent != null)
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
			Owner.Position = Owner.GetGlobalPosition() - target.GetGlobalPosition();
			Owner.Parent?.RemoveChild(Owner);
			target.AddChild(Owner, ChildManagementMode.Nothing, ChildTransformingMode.All);
			AnimateMove();
		}

		private void SuddenlyMoveCursor(Object2D target)
		{
			Owner.Position = Offset;
		}

		/// <summary>
		/// オーバーライドして、指定したオブジェクトへ移動するアニメーションを実装できます。
		/// </summary>
		/// <remarks>追従先のオブジェクトの子オブジェクトとなるため、(0, 0)の位置が追従先のオブジェクトの位置となります。</remarks>
		protected virtual void AnimateMove()
		{
			Owner.Position = Offset;
		}
	}
}
