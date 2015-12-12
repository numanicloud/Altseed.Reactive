using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.Linq;
using Nac.Altseed.ObjectSystem;

namespace Nac.Altseed.UI
{
	public class MessageWindow : ReactiveTextureObject2D
	{
		private Subject<Unit> onRead_ = new Subject<Unit>();
        private Func<bool> isReadKeyPushed;

		public TextObject2D TextObject { get; private set; }
		public TextureObject2D WaitIndicator { get; private set; }
		public float TextSpeed { get; set; }
		public IObservable<Unit> OnRead => onRead_;

		public MessageWindow()
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
		}

        public void SetReadControl<TAbstractKey>(Controller<TAbstractKey> controller, TAbstractKey readKey)
        {
            isReadKeyPushed = () => controller.GetState(readKey) == InputState.Push;
        }

		public async Task TalkMessageAsync(params string[] message)
		{
			await OnUpdateEvent.SelectCorourine(FlowToShowText(message, true));
		}

		public async Task TalkMessageWithoutReadAsync(string message)
		{
			await OnUpdateEvent.SelectCorourine(FlowToShowText(new string[] { message }, false));
		}

		public void ShowMessage(string message)
		{
			TextObject.Text = message;
		}

        public void Clear()
        {
            TextObject.Text = "";
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

					if(isReadKeyPushed?.Invoke() == true)
					{
						charCount = message.Length;
					}

					TextObject.Text = message.Substring(0, (int)charCount);
					yield return Unit.Default;
				}

				if(readKeyIsNecessary)
				{
					WaitIndicator.IsDrawn = true;
					while(isReadKeyPushed?.Invoke() != true)
					{
						yield return Unit.Default;
					}
					onRead_.OnNext(Unit.Default);
					WaitIndicator.IsDrawn = false;
				}

				yield return Unit.Default;
			}
		}
	}
}
