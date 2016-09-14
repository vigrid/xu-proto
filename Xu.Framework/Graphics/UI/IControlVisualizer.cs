using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xu.Graphics.UI.Controls;
using Xu.Graphics.UI.Layout;

namespace Xu.Graphics.UI
{
	public interface IControlVisualizer
	{
		void BeginBatch();
		void EndBatch();

		void DrawRectangle(Rectangle viewportRectangle, Color color);
		void DrawImage(Rectangle viewportRectangle, string imageAssetName, Color color);
		void DrawPanel(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor);
		void DrawProgressBar(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, Color progressBarColor, float progress);
		void DrawLabel(Rectangle viewportRectangle, string text, Label.LabelType labelType, Color textColor, Alignment textAlignment);
		void DrawButton(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, string text, Color textColor, Alignment textAlignment);
		void DrawTextBox(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, string text, Color textColor, Alignment textAlignment, int caretPosition, Color caretColor, bool autoWrap, int horizontalPadding, int verticalPadding);
		void DrawCheckBox(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, bool isChecked);
		void DrawRadioButton(Rectangle viewportRectangle, Color backgroundColor, Color outlineColor, bool isChecked);
		void DrawTexture(Rectangle viewportRectangle, Texture2D image, Color color);
	}
}
