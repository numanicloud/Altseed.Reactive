using Altseed.Reactive.Object;
using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Altseed.Reactive.Helper;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// テキスト表示を制御する機能を提供します。
	/// </summary>
	public class TextViewer : ReactiveTextureObject2D
	{
		private List<TextObject2D> Texts { get; set; }
		private Font Font { get; set; }

		/// <summary>
		/// TextViewerの新しいインスタンスを初期化します。
		/// </summary>
		public TextViewer(Font font)
		{
			Texts = new List<TextObject2D>();
			Font = font ?? Engine.Graphics.CreateDynamicFont("", 24, new Color(255, 255, 255), 1, new Color(0, 0, 0));
			IsDrawn = false;
		}

		/// <summary>
		/// 文字列を表示します。
		/// </summary>
		/// <param name="text">表示する文字列。</param>
		/// <param name="setting">文字列の表示方法。</param>
		public async Task ShowTextAsync(string text, TextSetting setting)
		{
			Vector2DF pos = new Vector2DF(0, 0);
			if (setting.NewPage)
			{
				Texts.ForEach(x => x.Dispose());
				Texts.Clear();
			}
			else if (Texts.Count > 0)
			{
				var prevObj = Texts[Texts.Count - 1];
				var size = Font.CalcTextureSize(prevObj.Text, WritingDirection.Horizontal).To2DF();
				if (setting.NewLine)
				{
					pos = new Vector2DF(0, prevObj.Position.Y + size.Y);
				}
				else
				{
					pos = prevObj.Position + new Vector2DF(size.X, 0);
				}
			}

			var obj = new TextObject2D()
			{
				Font = Font,
				Text = text.Substring(0, 1),
				Color = setting.TextColor,
				Position = pos,
			};
			AddChild(obj, ChildManagementMode.Disposal | ChildManagementMode.RegistrationToLayer,
				ChildTransformingMode.All);
			Texts.Add(obj);

			await Observable.Interval(TimeSpan.FromSeconds(1 / setting.TalkSpeed))
				.TakeWhile(t => t + 2 <= text.Length)
				.Do(t => obj.Text = text.Substring(0, (int)t + 2));
		}
	}
}
