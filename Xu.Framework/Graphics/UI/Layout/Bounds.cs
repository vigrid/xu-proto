namespace Xu.Graphics.UI.Layout
{
	public class Bounds
	{
		public Offset Left { get; private set; }
		public Offset Right { get; private set; }
		public Offset Top { get; private set; }
		public Offset Bottom { get; private set; }

		public static Bounds Fill()
		{
			return Fill(0, 0);
		}

		public static Bounds Fill(int padding)
		{
			return Fill(padding, padding);
		}

		public static Bounds Fill(int horizontalPadding, int verticalPadding)
		{
			return new Bounds
			{
				Left = new Offset(0.0f, horizontalPadding),
				Right = new Offset(1.0f, -horizontalPadding),
				Top = new Offset(0.0f, verticalPadding),
				Bottom = new Offset(1.0f, -verticalPadding),
			};
		}

		public static Bounds Column(int column, int maxColumns)
		{
			return Column(column, maxColumns, 0, 0);
		}

		public static Bounds Column(int column, int maxColumns, int padding)
		{
			return Column(column, maxColumns, padding, padding);
		}

		public static Bounds Column(int column, int maxColumns, int horizontalPadding, int verticalPadding)
		{
			return new Bounds
			{
				Left = new Offset(column / (float) maxColumns, horizontalPadding),
				Right = new Offset((column + 1) / (float) maxColumns, -horizontalPadding),
				Top = new Offset(0.0f, verticalPadding),
				Bottom = new Offset(1.0f, -verticalPadding),
			};
		}

		public static Bounds Row(int row, int maxRows)
		{
			return Row(row, maxRows, 0, 0);
		}

		public static Bounds Row(int row, int maxRows, int padding)
		{
			return Row(row, maxRows, padding, padding);
		}

		public static Bounds Row(int row, int maxRows, int horizontalPadding, int verticalPadding)
		{
			return new Bounds
			{
				Left = new Offset(0.0f, horizontalPadding),
				Right = new Offset(1.0f, -horizontalPadding),
				Top = new Offset(row / (float) maxRows, verticalPadding),
				Bottom = new Offset((row + 1) / (float) maxRows, -verticalPadding),
			};
		}

		public static Bounds Centered(int width, int height)
		{
			return new Bounds
			{
				Left = new Offset(0.5f, - width / 2),
				Right = new Offset(0.5f, width / 2),
				Top = new Offset(0.5f, - height / 2),
				Bottom = new Offset(0.5f, height / 2),
			};
		}

		public static Bounds DockLeft(int width)
		{
			return new Bounds
			{
				Left = new Offset(0.0f, 0),
				Right = new Offset(0.0f, width),
				Top = new Offset(0.0f, 0),
				Bottom = new Offset(1.0f, 0),
			};
		}

		public static Bounds DockRight(int width)
		{
			return new Bounds
			{
				Left = new Offset(1.0f, -width),
				Right = new Offset(1.0f, 0),
				Top = new Offset(0.0f, 0),
				Bottom = new Offset(1.0f, 0),
			};
		}

		public static Bounds DockTop(int height)
		{
			return new Bounds
			{
				Left = new Offset(0.0f, 0),
				Right = new Offset(1.0f, 0),
				Top = new Offset(0.0f, 0),
				Bottom = new Offset(0.0f, height),
			};
		}

		public static Bounds DockBottom(int height)
		{
			return new Bounds
			{
				Left = new Offset(0.0f, 0),
				Right = new Offset(1.0f, 0),
				Top = new Offset(1.0f, -height),
				Bottom = new Offset(1.0f, 0),
			};
		}

		public Bounds Translated(int horizontalValue, int verticalValue)
		{
			return new Bounds
			{
				Left = Left.Translated(horizontalValue),
				Right = Right.Translated(horizontalValue),
				Top = Top.Translated(verticalValue),
				Bottom = Bottom.Translated(verticalValue),
			};
		}

		public Bounds Translated(float horizontalFraction, float verticalFraction)
		{
			return new Bounds
			{
				Left = Left.Translated(horizontalFraction),
				Right = Right.Translated(horizontalFraction),
				Top = Top.Translated(verticalFraction),
				Bottom = Bottom.Translated(verticalFraction),
			};
		}

		public Bounds Expanded(int expansion)
		{
			return Expanded(expansion, expansion);
		}

		public Bounds Expanded(int horizontalExpansion, int verticalExpansion)
		{
			return new Bounds
			{
				Left = Left.Translated(-horizontalExpansion),
				Right = Right.Translated(horizontalExpansion),
				Top = Top.Translated(-verticalExpansion),
				Bottom = Bottom.Translated(verticalExpansion),
			};
		}

		public Bounds Expanded(int leftExpansion, int rightExpansion, int topExpansion, int bottomExpansion)
		{
			return new Bounds
			{
				Left = Left.Translated(-leftExpansion),
				Right = Right.Translated(rightExpansion),
				Top = Top.Translated(-topExpansion),
				Bottom = Bottom.Translated(bottomExpansion),
			};
		}
	}
}
