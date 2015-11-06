using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using asd;
using Nac.Altseed.Input;
using Nac.Altseed.Object2DComponents;

namespace Nac.Altseed.UI
{
	public class MessageWindow<TAbstractKey> : TextureObject2D
	{
		private Subject<System.Reactive.Unit> onRead_ = new Subject<System.Reactive.Unit>();

		public TextObject2D TextObject { get; private set; }
		public TextureObject2D WaitIndicator { get; private set; }

		public Controller<TAbstractKey> Controller { get; set; }
		public TAbstractKey ReadKey { get; set; }
		public float TextSpeed { get; set; }

		public IObservable<System.Reactive.Unit> OnRead => onRead_;

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

		public void TalkMessage(string[] message, Action callback)
		{
			// シーンに追加する方式だと親が居ないときに呼べない
			AddComponent(new CoroutineComponent(FlowToShowText(message, true), callback), "MessageWindow.TalkMessage");
		}

		public void TalkMessage(string message, Action callback)
		{
			TalkMessage(new string[] { message }, callback);
		}

		public void TalkMessageWithoutRead(string message, Action callback)
		{
			AddComponent(new CoroutineComponent(FlowToShowText(new string[] { message }, false), callback), "MessageWindow.TalkMessageWithoutRead");
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


		private IEnumerable<Unit> FlowToShowText(string[] text, bool readKeyIsNecessary)
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
					yield return Unit.I;
				}

				if(readKeyIsNecessary)
				{
					WaitIndicator.IsDrawn = true;
					while(Controller.GetState(ReadKey) != InputState.Push)
					{
						yield return Unit.I;
					}
					onRead_.OnNext(System.Reactive.Unit.Default);
					WaitIndicator.IsDrawn = false;
				}

				yield return Unit.I;
			}
		}
	}
}
