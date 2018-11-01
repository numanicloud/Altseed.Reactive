using Altseed.Reactive.Object;
using Altseed.Reactive.Ui.Attributes;
using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui.TextWindow
{
	/// <summary>
	/// エスケープシーケンスによって文字列スタイルを操作しながら、文字列の表示を制御するクラス。
	/// </summary>
	public class MarkupTextViewer : ReactiveTextureObject2D
	{
		private TextViewer TextViewer { get; set; }
		private IObservable<Unit> OnRead { get; set; }

		/// <summary>
		/// 文字列表示の基本となるスタイル設定。
		/// </summary>
		public TextSetting DefaultSetting;

		/// <summary>
		/// フォント設定を指定して、MarkupTextViewerの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="font">使用するフォント。</param>
		public MarkupTextViewer(Font font)
		{
			TextViewer = new TextViewer(font);
			IsDrawn = false;
			AddChild(TextViewer, ChildManagementMode.Disposal | ChildManagementMode.RegistrationToLayer,
				ChildTransformingMode.All);
		}

		public void BindControlToRead<TControl>(Input.Controller<TControl> controller, TControl control)
		{
			OnRead = OnUpdatedEvent.Where(t => controller.GetState(control) == Input.InputState.Release)
				.Select(t => Unit.Default);
		}

		/// <summary>
		/// エスケープシーケンスでスタイル付けされた文字列を表示します。
		/// </summary>
		/// <param name="text">エスケープシーケンスでスタイル付けされた文字列。</param>
		/// <returns>表示が終わると完了するタスク。</returns>
		public async Task ShowStyledText(string text)
		{
			var style = DefaultSetting;

			var normalized = text.Replace("\n", "\\n").Replace("\r", "\\r");
			var matches1 = Regex.Matches(normalized,
				@"(?<attr>\\[npcskrw](\[[^\\]+\])?)*(?<str>[^\\]+)");

			foreach (Match match in matches1)
			{
				var localStyle = style;

				var attrCaptures = match.Groups["attr"].Captures;
				for (int i = 0; i < attrCaptures.Count; i++)
				{
					var matches2 = Regex.Matches(attrCaptures[i].Value,
						@"(?<name>\\[npcskrw])(\[(?<exp>[^\\]+)\])?");
					if (matches2.Count == 0)
					{
						continue;
					}

					var name = matches2[0].Groups["name"].Captures[0].Value;
					if (name == "\\k")
					{
						await OnRead.Take(1);
						continue;
					}

					var attr = AttributeFacade.Instance.GetAttribute(name);
					if (attr == null)
					{
						continue;
					}

					var exp = "";
					if (matches2[0].Groups["exp"].Captures.Count > 0)
					{
						exp = matches2[0].Groups["exp"].Captures[0].Value;
					}

					localStyle = attr.ModifySetting(localStyle, exp);
					if (attr.IsContinue)
					{
						style = attr.ModifySetting(style, exp);
					}

					await attr.BeforeShowText(exp);
				}

				await TextViewer.ShowTextAsync(match.Groups["str"].Value, localStyle);
			}
		}
	}
}
