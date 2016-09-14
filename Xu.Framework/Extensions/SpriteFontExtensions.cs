using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Extensions
{
	public static class SpriteFontExtensions
	{
		private static readonly Lazy<StringBuilder> LineText = new Lazy<StringBuilder>(() => new StringBuilder());

		public static List<string> WrapText(this SpriteFont font, string text, float lineWidth)
		{
			var lineText = LineText.Value;

			var list = new List<string>();
			foreach (string line in text.Split('\n'))
			{
				lineText.Clear();
				if (line != string.Empty)
				{
					string[] words = line.Split(' ');
					foreach (string word in words)
					{
						if (lineText.Length > 0)
						{
							lineText.Append(" ");
						}
						if (lineText.Length > 0 && font.MeasureString(lineText + word).X > lineWidth)
						{
							list.Add(lineText.ToString());
							lineText.Clear();
						}
						lineText.Append(word);
					}
				}
				list.Add(lineText + " ");
			}
			return list;
		}
	}
}
