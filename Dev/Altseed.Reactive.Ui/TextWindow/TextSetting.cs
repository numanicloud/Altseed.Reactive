using asd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altseed.Reactive.Ui
{
	/// <summary>
	/// テキストの制御設定を保持する構造体。
	/// </summary>
	public struct TextSetting
	{
		/// <summary>
		/// テキストの色。
		/// </summary>
		public asd.Color TextColor;
		/// <summary>
		/// テキストを表示する速度。
		/// </summary>
		public float TalkSpeed;
		/// <summary>
		/// このテキストの前に改行をするかどうか。
		/// </summary>
		public bool NewLine;
		/// <summary>
		/// このテキストの前に改ページをするかどうか。
		/// </summary>
		public bool NewPage;

		/// <summary>
		/// 改行を設定し、それ以外はこのインスタンスと同じ設定を返します。
		/// </summary>
		/// <returns>改行を設定されたインスタンス。</returns>
		public TextSetting WithNewLine() => new TextSetting
		{
			NewLine = true,
			NewPage = NewPage,
			TalkSpeed = TalkSpeed,
			TextColor = TextColor,
		};

		/// <summary>
		/// 改ページを設定し、それ以外はこのインスタンスと同じ設定を返します。
		/// </summary>
		/// <returns>改ページを設定されたインスタンス。</returns>
		public TextSetting WithNewPage() => new TextSetting
		{
			NewLine = NewLine,
			NewPage = true,
			TalkSpeed = TalkSpeed,
			TextColor = TextColor,
		};

		/// <summary>
		/// 色を設定し、それ以外はこのインスタンスと同じ設定を返します。
		/// </summary>
		/// <param name="color">新しい色。</param>
		/// <returns>色を設定されたインスタンス。</returns>
		public TextSetting WithColor(Color color) => new TextSetting
		{
			NewLine = NewLine,
			NewPage = NewPage,
			TalkSpeed = TalkSpeed,
			TextColor = color,
		};

		/// <summary>
		/// テキストを表示する速度を設定し、それ以外はこのインスタンスと同じ設定を返します。
		/// </summary>
		/// <param name="speed">設定する速度。</param>
		/// <returns>テキストを表示する速度を設定されたインスタンス。</returns>
		public TextSetting WithSpeed(float speed)
		{
			return new TextSetting()
			{
				NewLine = NewLine,
				NewPage = NewPage,
				TextColor = TextColor,
				TalkSpeed = speed,
			};
		}
	}
}
