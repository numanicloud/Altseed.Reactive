using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
    class CollectionBinderTest : AltseedTest
    {
        class ListItem : TextObject2D, IActivatableSelectionItem
        {
            public ListItem()
            {
                Color = new Color(240, 196, 90, 255);
            }

            public void Activate()
            {
                Color = new Color(0, 0, 0, 255);
            }

            public void Disactivate()
            {
                Color = new Color(240, 196, 90, 255);
            }
        }
        
        ObservableCollection<int> collection;
        Font font;

        protected override void OnStart()
        {
            collection = new ObservableCollection<int>();

            var layout = new LinearPanel()
            {
                ItemSpan = new Vector2DF(0, 36),
            };

            var selector = new Selector<int, Control>(CreateController(), layout);
            selector.Cursor.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
            selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);
            selector.Loop = true;
			selector.RegisterLayer((Layer2D)Engine.CurrentScene.Layers.First());

            font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
            CollectionBinderForSelector<int>.Bind(selector, collection, c => new ListItem()
            {
                Font = font,
                Text = $"追加アイテム{c}",
            }, false);
        }

        protected override void OnUpdate()
        {
            if(Engine.Keyboard.GetKeyState(Keys.Q) == KeyState.Push)
            {
                var index = collection.Any() ? collection.Max() + 1 : 0;
                collection.Add(index);
            }
            else if(collection.Skip(2).Any() && Engine.Keyboard.GetKeyState(Keys.W) == KeyState.Push)
            {
                collection.RemoveAt(2);
            }
        }
    }
}
