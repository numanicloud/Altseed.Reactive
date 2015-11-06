using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;

namespace Nac.Altseed.Test
{
	class ReactiveInputTest : AltseedTest
	{
		private Subject<System.Reactive.Unit> onUpdateObservable = new Subject<System.Reactive.Unit>();

		public ReactiveInputTest() : base("ReactiveInput")
		{
		}

		protected override void OnInitialize()
		{
			var font = Engine.Graphics.CreateDynamicFont("", 28, new Color(255, 255, 255, 255), 1, new Color(0, 0, 0, 255));
            var obj = new TextObject2D()
			{
				Font = font,
				Text = "Default",
			};
			Engine.AddObject2D(obj);

			int count = 0;
			var obj2 = new TextObject2D()
			{
				Font = font,
				Text = count.ToString(),
				Position = new Vector2DF(0, 40),
			};
			Engine.AddObject2D(obj2);
			
			var keyboard = new KeyboardController<int>();
			keyboard.BindKey(asd.Keys.Enter, 0);

			var keyboard2 = new KeyboardController<int>();
			keyboard2.BindKey(asd.Keys.Space, 0);

			var input1 = onUpdateObservable.Select(x => keyboard.GetState(0));
			var input2 = onUpdateObservable.Select(x => keyboard2.GetState(0));

			input1.Join(input2,
				x => Observable.Never<Unit>(),
				x => Observable.Never<Unit>(),
				(l, r) => Merge(l, r))
				.Subscribe(x => obj.Text = x?.ToString());

			var hold = onUpdateObservable.Select(x => keyboard.GetState(0))
				.Where(x => x == InputState.Push || x == InputState.Hold);

			hold.Take(1)
				.Merge(hold.Buffer(30).Select(x => x.First()))
				.Subscribe(x =>
				{
					count++;
					obj2.Text = count.ToString();
				});

			/*
			onUpdateObservable.Select(x => keyboard.GetState(0))
				.Where(x => x == InputState.Push || x == InputState.Hold)
				.Subscribe(x => Console.WriteLine("Input: " + x));
			//*/
		}

		private InputState? Merge(params InputState?[] states)
		{
			if(states.Any(x => x == InputState.Hold))
			{
				return InputState.Hold;
			}
			else if(states.Any(x => x == InputState.Push))
			{
				return InputState.Push;
			}
			else if(states.Any(x => x == InputState.Release))
			{
				return InputState.Release;
			}
			else if(states.Any(x => x == InputState.Free))
			{
				return InputState.Free;
			}
			else
			{
				return null;
			}
		}

		protected override void OnUpdate()
		{
			onUpdateObservable.OnNext(System.Reactive.Unit.Default);
		}
	}
}
