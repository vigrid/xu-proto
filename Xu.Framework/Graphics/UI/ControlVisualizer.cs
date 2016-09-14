using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xu.Extensions;
using Xu.Graphics.UI.Controls;
using Xu.Graphics.UI.Layout;

namespace Xu.Graphics.UI
{
	public class ControlVisualizer : IControlVisualizer, IDisposable
	{
		private readonly Game _game;
		private SpriteBatch _spriteBatch;
		private Texture2D _hudAtlas;
		private Dictionary<UIElement, Rectangle> _hudAtlasCoords;
		private SpriteFont _mainFont;
		private bool _needsResources = true;
		private SpriteFont _titleFont;
		private Texture2D _whiteDot;

		public ControlVisualizer(Game game)
		{
			_game = game;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_spriteBatch.Dispose();
		}

		#endregion

		#region IControlVisualizer Members

		public void DrawRectangle(Rectangle viewportRectangle, Color color)
		{
			_spriteBatch.Draw(_whiteDot, viewportRectangle, color);
		}

		public void DrawImage(Rectangle viewportRectangle, string imageAssetName, Color color)
		{
			_spriteBatch.Draw(_game.Content.Load<Texture2D>(imageAssetName), viewportRectangle, color);
		}

		public void DrawPanel(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor)
		{
			if (backgroundColor != Color.Transparent)
			{
				DrawControlBackgroundMosaic(viewportRectangle, backgroundColor);
			}

			if (outlineColor != Color.Transparent)
			{
				DrawControlOutlineMosaic(viewportRectangle, outlineColor);
			}
		}

		public void DrawProgressBar(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, Color progressBarColor, float progress)
		{
			if (backgroundColor != Color.Transparent)
			{
				DrawControlBackgroundMosaic(viewportRectangle, backgroundColor);
			}

			if (progressBarColor != Color.Transparent)
			{
				Rectangle progressRectangle = viewportRectangle;
				progressRectangle.Width = (int) (progress * progressRectangle.Width);
				DrawControlBackgroundMosaic(progressRectangle, progressBarColor);
			}

			if (outlineColor != Color.Transparent)
			{
				DrawControlOutlineMosaic(viewportRectangle, outlineColor);
			}
		}

		public void DrawLabel(Rectangle viewportRectangle, string text, Label.LabelType labelType, Color textColor, Alignment textAlignment)
		{
			if (!String.IsNullOrWhiteSpace(text) && textColor != Color.Transparent)
			{
				switch (labelType)
				{
					case Label.LabelType.Title:
						DrawString(_titleFont, text, viewportRectangle, textColor, textAlignment ?? Alignment.None);
						break;
					case Label.LabelType.Main:
						DrawString(_mainFont, text, viewportRectangle, textColor, textAlignment ?? Alignment.None);
						break;
					default:
						throw new ArgumentOutOfRangeException("labelType");
				}
			}
		}

		public void DrawButton(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, string text, Color textColor, Alignment textAlignment)
		{
			DrawPanel(viewportRectangle, backgroundColor, outlineColor);
			DrawLabel(viewportRectangle, text, Label.LabelType.Main, textColor, textAlignment);
		}

		public void DrawTextBox(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, string text, Color textColor, Alignment textAlignment, int caretPosition, Color caretColor, bool autoWrap, int horizontalPadding, int verticalPadding)
		{
			DrawPanel(viewportRectangle, backgroundColor, outlineColor);

			viewportRectangle.X += horizontalPadding;
			viewportRectangle.Y += verticalPadding;
			viewportRectangle.Width -= horizontalPadding * 2;
			viewportRectangle.Height -= verticalPadding * 2;

			List<string> lines = null;
			if (autoWrap)
			{
				lines = _mainFont.WrapText(text, viewportRectangle.Width);
				text = String.Join("\n", lines);
			}
			DrawString(_mainFont, text, viewportRectangle, textColor, textAlignment ?? Alignment.None);

			if (caretPosition != -1)
			{
				if (lines == null)
				{
					DrawCaret(_mainFont, text, viewportRectangle, caretColor, caretPosition, textAlignment);
				}
				else
				{
					DrawCaretMultiline(_mainFont, text, viewportRectangle, caretColor, caretPosition, textAlignment, lines);
				}
			}
		}

		public void DrawCheckBox(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, bool isChecked)
		{
			DrawPanel(viewportRectangle, backgroundColor, outlineColor);
			if (isChecked)
			{
				DrawWidget(UIElement.WidgetCross, viewportRectangle, Alignment.Center);
			}
		}

		public void DrawRadioButton(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, bool isChecked)
		{
			DrawPanel(viewportRectangle, backgroundColor, outlineColor);
			DrawWidget(isChecked ? UIElement.WidgetCircleFilled : UIElement.WidgetCircleEmpty, viewportRectangle, Alignment.Center);
		}

		public void DrawTexture(Rectangle viewportRectangle, Texture2D image, Color color)
		{
			_spriteBatch.Draw(image, viewportRectangle, color);
		}

		public void BeginBatch()
		{
			EnsureResources();

			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		}

		public void EndBatch()
		{
			_spriteBatch.End();
		}

		#endregion

		private void DrawString(SpriteFont font, string text, Rectangle viewportRectangle, Color textColor, Alignment alignment)
		{
			Vector2 textSize = font.MeasureString(text);
			var textRectangle = new Rectangle(viewportRectangle.X, viewportRectangle.Y, (int) Math.Ceiling(textSize.X), (int) Math.Ceiling(textSize.Y));
			Rectangle destinationRectangle = alignment.Adjust(viewportRectangle, textRectangle);
			var textPosition = new Vector2(destinationRectangle.X, destinationRectangle.Y);

			_spriteBatch.DrawString(font, text, textPosition, textColor);
		}

		private void DrawCaret(SpriteFont font, string text, Rectangle viewportRectangle, Color caretColor, int caretPosition, Alignment textAlignment)
		{
			DrawCaretMultiline(font, text, viewportRectangle, caretColor, caretPosition, textAlignment, new List<string> { text });
		}

		private void DrawCaretMultiline(SpriteFont font, string text, Rectangle viewportRectangle, Color caretColor, int caretPosition, Alignment textAlignment, List<string> lines)
		{
			if (text != String.Join("\n", lines) || caretPosition > text.Length)
			{
				throw new InvalidOperationException("This should never happen");
			}

			int caretLine;
			int previousLinesLength = 0;
			for (caretLine = 0; caretLine < lines.Count; caretLine++)
			{
				if (caretPosition < previousLinesLength + lines[caretLine].Length)
				{
					break;
				}
				previousLinesLength += lines[caretLine].Length + 1;
			}
			int lineCaretPosition = caretPosition - previousLinesLength + caretLine;

			Vector2 textSize = font.MeasureString(text);
			if (String.IsNullOrEmpty(text))
			{
				textSize.Y = font.LineSpacing;
			}
			Vector2 textSizeToCaret = font.MeasureString(lines[caretLine].Substring(0, lineCaretPosition));
			var textRectangle = new Rectangle(viewportRectangle.X, viewportRectangle.Y, (int) Math.Ceiling(textSize.X), (int) Math.Ceiling(textSize.Y));
			Rectangle textDestinationRectangle = textAlignment.Adjust(viewportRectangle, textRectangle);
			var caretRectangle = new Rectangle((int) (textDestinationRectangle.X + textSizeToCaret.X), textDestinationRectangle.Y + caretLine * font.LineSpacing, 2, font.LineSpacing);

			_spriteBatch.Draw(_whiteDot, caretRectangle, caretColor);
		}

		private void DrawControlOutlineMosaic(Rectangle viewportRectangle, Color color)
		{
			int lw = _hudAtlasCoords[UIElement.OutlineLeft].Width;
			int rw = _hudAtlasCoords[UIElement.OutlineRight].Width;
			int th = _hudAtlasCoords[UIElement.OutlineTop].Height;
			int bh = _hudAtlasCoords[UIElement.OutlineBottom].Height;

			viewportRectangle.Width = Math.Max(lw + rw, viewportRectangle.Width);
			viewportRectangle.Height = Math.Max(th + bh, viewportRectangle.Height);

			int cw = viewportRectangle.Width - lw - rw;
			int ch = viewportRectangle.Height - th - bh;

			int x0 = viewportRectangle.Left;
			int x1 = x0 + lw;
			int x2 = x1 + cw;
			int y0 = viewportRectangle.Top;
			int y1 = y0 + th;
			int y2 = y1 + ch;

			DrawTile(UIElement.OutlineTopLeft, x0, y0, color);
			DrawTile(UIElement.OutlineTop, x1, y0, cw, th, color);
			DrawTile(UIElement.OutlineTopRight, x2, y0, color);
			DrawTile(UIElement.OutlineLeft, x0, y1, lw, ch, color);
			DrawTile(UIElement.OutlineCenter, x1, y1, cw, ch, color);
			DrawTile(UIElement.OutlineRight, x2, y1, rw, ch, color);
			DrawTile(UIElement.OutlineBottomLeft, x0, y2, color);
			DrawTile(UIElement.OutlineBottom, x1, y2, cw, bh, color);
			DrawTile(UIElement.OutlineBottomRight, x2, y2, color);
		}

		private void DrawControlBackgroundMosaic(Rectangle viewportRectangle, Color color)
		{
			int lw = _hudAtlasCoords[UIElement.BackgroundLeft].Width;
			int rw = _hudAtlasCoords[UIElement.BackgroundRight].Width;
			int th = _hudAtlasCoords[UIElement.BackgroundTop].Height;
			int bh = _hudAtlasCoords[UIElement.BackgroundBottom].Height;

			viewportRectangle.Width = Math.Max(lw + rw, viewportRectangle.Width);
			viewportRectangle.Height = Math.Max(th + bh, viewportRectangle.Height);

			int cw = viewportRectangle.Width - lw - rw;
			int ch = viewportRectangle.Height - th - bh;

			int x0 = viewportRectangle.Left;
			int x1 = x0 + lw;
			int x2 = x1 + cw;
			int y0 = viewportRectangle.Top;
			int y1 = y0 + th;
			int y2 = y1 + ch;

			DrawTile(UIElement.BackgroundTopLeft, x0, y0, color);
			DrawTile(UIElement.BackgroundTop, x1, y0, cw, th, color);
			DrawTile(UIElement.BackgroundTopRight, x2, y0, color);
			DrawTile(UIElement.BackgroundLeft, x0, y1, lw, ch, color);
			DrawTile(UIElement.BackgroundCenter, x1, y1, cw, ch, color);
			DrawTile(UIElement.BackgroundRight, x2, y1, rw, ch, color);
			DrawTile(UIElement.BackgroundBottomLeft, x0, y2, color);
			DrawTile(UIElement.BackgroundBottom, x1, y2, cw, bh, color);
			DrawTile(UIElement.BackgroundBottomRight, x2, y2, color);
		}

		private void DrawWidget(UIElement uiElement, Rectangle viewportRectangle, Alignment alignment)
		{
			Rectangle source = _hudAtlasCoords[uiElement];
			Rectangle destination = Alignment.Center.Adjust(viewportRectangle, source);
			DrawTile(destination, source, Color.White);
		}

		private void DrawTile(UIElement uiElement, int x, int y, Color color)
		{
			Rectangle source = _hudAtlasCoords[uiElement];
			var destination = new Rectangle(x, y, source.Width, source.Height);
			DrawTile(destination, source, color);
		}

		private void DrawTile(UIElement uiElement, int x, int y, int width, int height, Color color)
		{
			Rectangle source = _hudAtlasCoords[uiElement];
			var destination = new Rectangle(x, y, width, height);
			DrawTile(destination, source, color);
		}

		private void DrawTile(Rectangle destination, Rectangle source, Color color)
		{
			_spriteBatch.Draw(_hudAtlas, destination, source, color);
		}

		private Dictionary<UIElement, Rectangle> GenerateElementAtlasCoords()
		{
			// TODO: Un-hardcode, ideally read from XML

			var coords = new Dictionary<UIElement, Rectangle>();

			coords.Add(UIElement.BackgroundTopLeft, new Rectangle(0, 0, 8, 8));
			coords.Add(UIElement.BackgroundTop, new Rectangle(8, 0, 8, 8));
			coords.Add(UIElement.BackgroundTopRight, new Rectangle(16, 0, 8, 8));
			coords.Add(UIElement.BackgroundLeft, new Rectangle(0, 8, 8, 8));
			coords.Add(UIElement.BackgroundCenter, new Rectangle(8, 8, 8, 8));
			coords.Add(UIElement.BackgroundRight, new Rectangle(16, 8, 8, 8));
			coords.Add(UIElement.BackgroundBottomLeft, new Rectangle(0, 16, 8, 8));
			coords.Add(UIElement.BackgroundBottom, new Rectangle(8, 16, 8, 8));
			coords.Add(UIElement.BackgroundBottomRight, new Rectangle(16, 16, 8, 8));

			coords.Add(UIElement.OutlineTopLeft, new Rectangle(0, 24, 8, 8));
			coords.Add(UIElement.OutlineTop, new Rectangle(8, 24, 8, 8));
			coords.Add(UIElement.OutlineTopRight, new Rectangle(16, 24, 8, 8));
			coords.Add(UIElement.OutlineLeft, new Rectangle(0, 32, 8, 8));
			coords.Add(UIElement.OutlineCenter, new Rectangle(8, 32, 8, 8));
			coords.Add(UIElement.OutlineRight, new Rectangle(16, 32, 8, 8));
			coords.Add(UIElement.OutlineBottomLeft, new Rectangle(0, 40, 8, 8));
			coords.Add(UIElement.OutlineBottom, new Rectangle(8, 40, 8, 8));
			coords.Add(UIElement.OutlineBottomRight, new Rectangle(16, 40, 8, 8));

			coords.Add(UIElement.WidgetCross, new Rectangle(24, 0, 8, 8));
			coords.Add(UIElement.WidgetCircleEmpty, new Rectangle(32, 0, 8, 8));
			coords.Add(UIElement.WidgetCircleFilled, new Rectangle(40, 0, 8, 8));

			return coords;
		}

		private void EnsureResources()
		{
			if (_needsResources)
			{
				_whiteDot = _game.Content.Load<Texture2D>("UI/Dot-White");
				_hudAtlas = _game.Content.Load<Texture2D>("UI/HUD");
				_titleFont = _game.Content.Load<SpriteFont>("UI/font-title");
				_mainFont = _game.Content.Load<SpriteFont>("UI/font-main");
				_hudAtlasCoords = GenerateElementAtlasCoords();
				_spriteBatch = new SpriteBatch(_game.GraphicsDevice);

				_needsResources = false;
			}
		}
	}
}
