using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;
using Nac.Altseed.Reactive.UI;

namespace Nac.Altseed.Reactive.Test
{
    class SelectorTest : AltseedTest
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
        
        protected override void OnStart()
        {
            var controller = new KeyboardController<int>();
            controller.BindKey(Keys.Down, 0);
            controller.BindKey(Keys.Up, 1);
            controller.BindKey(Keys.Z, 2);
            controller.BindKey(Keys.X, 3);

            var selector = new Selector<int, int>(controller);
            selector.Texture = Engine.Graphics.CreateTexture2D("ListCursor.png");
            selector.BindKey(0, 1, 2, 3);
            Engine.AddObject2D(selector);
            selector.Loop = true;
            selector.IsActive = true;

            var font = Engine.Graphics.CreateDynamicFont("", 20, new Color(255, 255, 255, 255), 0, new Color(0, 0, 0, 255));
            var itemList = new List<ListItem>();
            for(int i = 0; i < 8; i++)
            {
                var obj = new ListItem()
                {
                    Text = $"選択肢{i}",
                    Font = font,
                    Position = new Vector2DF(10, 10 + i * 36),
                };
                itemList.Add(obj);
                Engine.AddObject2D(obj);
                selector.AddChoice(i, obj);
            }

            var moveSound = Engine.Sound.CreateSoundSource("kachi38.wav", true);
            var decideSound = Engine.Sound.CreateSoundSource("pi78.wav", true);
            var cancelSound = Engine.Sound.CreateSoundSource("pi11.wav", true);

            selector.OnSelectionChanged.Subscribe(i => Engine.Sound.Play(moveSound));
            selector.OnDecide.Subscribe(i =>
            {
                Engine.Sound.Play(decideSound);
                selector.IsActive = false;
            });
            selector.OnCancel.Subscribe(i =>
            {
                selector.ChoiceItems.Skip(2).FirstOrDefault()?.Item.Vanish();
            });
        }
    }
}
