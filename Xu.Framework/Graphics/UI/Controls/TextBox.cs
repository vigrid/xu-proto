using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xu.Graphics.UI.Layout;
using Xu.Input;

namespace Xu.Graphics.UI.Controls
{
	public class TextBox : Control, IKeyConsumer
	{
		public void Consume(KeyboardBufferItem key)
		{
			if (key.Character.HasValue)
			{
				InsertChar(key.Character.Value);
			}
			else
			{
				HandleKey(key.VirtualKey.Value, key.ActiveModifiers);
			}
		}

		private readonly StringBuilder _text = new StringBuilder(64);
		public int CaretPosition { get; private set; }
		public bool AutoWrap { get; set; }

		public string Text
		{
			get { return _text.ToString(); }
			set
			{
				_text.Clear();
				_text.Append(value);
				CaretPosition = _text.Length;
			}
		}

		public Color BackgroundColor { get; set; }
		public Color OutlineColor { get; set; }
		public Color TextColor { get; set; }
		public Alignment TextAlignment { get; set; }
		public Color CaretColor { get; set; }
		public int HorizontalTextPadding { get; set; }
		public int VerticalTextPadding { get; set; }

		public override void Draw(IControlVisualizer visualizer, GameTime gameTime)
		{
			visualizer.DrawTextBox(ViewportRectangle, BackgroundColor, OutlineColor, Text, TextColor, TextAlignment, HasFocus ? CaretPosition : -1, CaretColor, AutoWrap, HorizontalTextPadding, VerticalTextPadding);
		}

		private void InsertChar(char value)
		{
			_text.Insert(CaretPosition++, value);
		}

		private void HandleKey(Keys value, KeyboardBufferItem.Modifiers activeModifiers)
		{
			switch (value)
			{
				case Keys.Left:
					CaretPosition = Math.Max(0, CaretPosition - 1);
					break;
				case Keys.Right:
					CaretPosition = Math.Min(_text.Length, CaretPosition + 1);
					break;
				case Keys.Home:
					CaretPosition = 0;
					break;
				case Keys.End:
					CaretPosition = _text.Length;
					break;
				case Keys.Back:
					if (CaretPosition > 0)
					{
						CaretPosition = Math.Max(0, CaretPosition - 1);
						_text.Remove(CaretPosition, 1);
					}
					break;
				case Keys.Delete:
					if (CaretPosition < _text.Length)
					{
						_text.Remove(CaretPosition, 1);
					}
					break;
				case Keys.Enter:
				case Keys.Escape:
				case Keys.Tab:
					DoFocusLose();
					break;
			}
		}
	}
}
