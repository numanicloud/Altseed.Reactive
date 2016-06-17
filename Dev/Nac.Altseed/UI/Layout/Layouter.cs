using System.Collections.Generic;
using asd;

namespace Nac.Altseed.UI
{
	/// <summary>
	/// オブジェクトを何らかの配置に並べる機能を提供します。
	/// </summary>
	public abstract class Layouter : ObjectSystem.ReactiveTextureObject2D
	{
		/// <summary>
		/// 2Dオブジェクトをこのレイアウトの末尾に配置します。
		/// </summary>
		/// <param name="item">配置する2Dオブジェクト。</param>
		public abstract void AddItem(Object2D item);

		/// <summary>
		/// 2Dオブジェクトをこのレイアウト上に挿入します。
		/// </summary>
		/// <param name="index">挿入する位置のインデックス。</param>
		/// <param name="item">挿入する2Dオブジェクト。</param>
		public abstract void InsertItem(int index, Object2D item);

		/// <summary>
		/// 2Dオブジェクトをこのレイアウトから取り除きます。
		/// </summary>
		/// <param name="item">取り除く2Dオブジェクト。</param>
		public abstract void RemoveItem(Object2D item);

		/// <summary>
		/// このレイアウトから全ての要素を取り除きます。
		/// </summary>
		public abstract void ClearItem();

        protected abstract IEnumerable<Object2D> ObjectsInternal { get; }

        protected override void OnUpdate()
        {
            var beVanished = new List<Object2D>();
            foreach(var item in ObjectsInternal)
            {
                if(!item.IsAlive)
                {
                    beVanished.Add(item);
                }
            }
            beVanished.ForEach(x => RemoveItem(x));
			base.OnUpdate();
        }
    }
}
