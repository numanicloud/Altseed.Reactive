using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.UI;

namespace Nac.Altseed.Reactive.Test
{
    class MenuTest : AltseedTest
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

        int count = 0;
        ObservableCollection<int> choices;
        Menu<int, Control> menu;
        Font font;

        protected override void OnStart()
        {
            choices = new ObservableCollection<int>();
            menu = new Menu<int, Control>()
            {
                Layouter = new LinearPanel()
                {
                    ItemSpan = new Vector2DF(0, 20),
                },
                Selector = new Selector<int, Control>(CreateController())
                {
                    Texture = Engine.Graphics.CreateTexture2D("ListCursor.png"),
                },
            };
            Engine.AddObject2D(menu);
            menu.Selector.BindKey(Control.Down, Control.Up, Control.Decide, Control.Cancel);

            // これは忘れやすい
            menu.Selector.IsActive = true;

            font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
        }

        protected override void OnUpdate()
        {
            if(Engine.Keyboard.GetKeyState(Keys.Enter) == KeyState.Push)
            {
                choices.Add(count);
                var obj = new ListItem()
                {
                    Font = font,
                    Text = $"アイテム{count}"
                };
                menu.AddChoice(count, obj);
                Engine.AddObject2D(obj);
                count++;
            }
            if(choices.Count > 3 && Engine.Keyboard.GetKeyState(Keys.Escape) == KeyState.Push)
            {
                choices.RemoveAt(3);
                menu.Selector.ChoiceItems[3].Item.Vanish();
            }
        }
    }
}
