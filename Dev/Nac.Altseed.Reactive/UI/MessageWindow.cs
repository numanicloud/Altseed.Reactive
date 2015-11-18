using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Reactive.Input;

namespace Nac.Altseed.Reactive.UI
{
	public class MessageWindow<TAbstractKey> : TextureObject2D
	{
		private Subject<Unit> onRead_ = new Subject<Unit>();

		public TextObject2D TextObject { get; private set; }
		public TextureObject2D WaitIndicator { get; private set; }

		public Controller<TAbstractKey> Controller { get; set; }
		public TAbstractKey ReadKey { get; set; }
		public float TextSpeed { get; set; }

		public IObservable<Unit> OnRead => onRead_;

		public MessageWindow(Controller<TAbstractKey> controller)
		{
			TextObject = new TextObject2D()
			{
				Text = "",
			};
			WaitIndicator = new TextureObject2D()
			{
				IsDrawn = false,
			};
			TextSpeed = 1;
			Controller = controller;
		}

		public async Task TalkMessageAsync(string[] message)
		{
            await Observable.FromCoroutine(FlowToShowText(message, true));
		}

		public async Task TalkMessageAsync(string message)
		{
			await TalkMessageAsync(new string[] { message });
		}

		public async Task TalkMessageWithoutReadAsync(string message)
		{
            await Observable.FromCoroutine(FlowToShowText(new string[] { message }, false));
		}

		public void ShowMessage(string message)
		{
			TextObject.Text = message;
		}


		protected override void OnStart()
		{
			TextObject.DrawingPriority = DrawingPriority + 1;
			AddChild(TextObject, ChildMode.Position);
			Layer.AddObject(TextObject);

			WaitIndicator.DrawingPriority = DrawingPriority + 1;
			TextObject.AddChild(WaitIndicator, ChildMode.Position);
			Layer.AddObject(WaitIndicator);
		}

		protected override void OnVanish()
		{
			TextObject.Vanish();
			WaitIndicator.Vanish();
		}
       
		private IEnumerator<Unit> FlowToShowText(string[] text, bool readKeyIsNecessary)
		{
			foreach(var message in text)
			{
				float charCount = 0;
				while(charCount < message.Length)
				{
					charCount += TextSpeed;

					if(Controller.GetState(ReadKey) == InputState.Push)
					{
						charCount = message.Length;
					}

					TextObject.Text = message.Substring(0, (int)charCount);
					yield return Unit.Default;
				}

				if(readKeyIsNecessary)
				{
					WaitIndicator.IsDrawn = true;
					while(Controller.GetState(ReadKey) != InputState.Push)
					{
						yield return Unit.Default;
					}
					onRead_.OnNext(System.Reactive.Unit.Default);
					WaitIndicator.IsDrawn = false;
				}

				yield return Unit.Default;
			}
		}
	}
}
