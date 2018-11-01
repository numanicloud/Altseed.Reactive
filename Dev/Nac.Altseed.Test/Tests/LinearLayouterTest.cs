using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.UI;

namespace Nac.Altseed.Test
{
    class LinearLayouterTest : AltseedTest
    {
        LinearPanel layout;
        Font font;
        
        protected override void OnStart()
        {
            layout = new LinearPanel();
            Engine.AddObject2D(layout);

            font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
            for(int i = 0; i < 7; i++)
            {
                var obj = new TextObject2D()
                {
                    Font = font,
                    Text = $"選択肢{i}",
                };
                layout.AddItem(obj);
                Engine.AddObject2D(obj);
            }

            layout.StartingOffset = new Vector2DF(10, 10);
            layout.ItemSpan = new Vector2DF(5, 20);
        }

        protected override void OnUpdate()
        {
            if(Engine.Keyboard.GetKeyState(Keys.Q) == ButtonState.Push)
            {
                var obj = new TextObject2D()
                {
                    Font = font,
                    Text = "追加アイテム",
                };
                layout.AddItem(obj);
                Engine.AddObject2D(obj);
            }
            else if(Engine.Keyboard.GetKeyState(Keys.W) == ButtonState.Push && layout.Items.Skip(3).Any())
            {
                var obj = new TextObject2D()
                {
                    Font = font,
                    Text = "挿入アイテム",
                };
                layout.InsertItem(3, obj);
                Engine.AddObject2D(obj);
            }

            if(Engine.Keyboard.GetKeyState(Keys.LeftAlt) == ButtonState.Hold)
            {
                if(Engine.Keyboard.GetKeyState(Keys.E) == ButtonState.Push)
                {
                    var item = layout.Items.Skip(2).FirstOrDefault();
                    if(item != null)
                    {
                        layout.RemoveItem(item.Object);
                    }
                }
                else if(Engine.Keyboard.GetKeyState(Keys.R) == ButtonState.Push)
                {
                    layout.ClearItem();
                }
            }
            else
            {
                if(Engine.Keyboard.GetKeyState(Keys.E) == ButtonState.Push)
                {
                    layout.Items.Skip(2).FirstOrDefault()?.Object?.Dispose();
                }
                else if(Engine.Keyboard.GetKeyState(Keys.R) == ButtonState.Push)
                {
                    foreach(var item in layout.Items)
                    {
                        item.Object.Dispose();
                    }
                }
            }
        }
    }
}
