using System;
using Microsoft.Xna.Framework;

namespace Xu.Graphics.UI.Layout
{
	public class Alignment
	{
		private static readonly Alignment _none;
		private static readonly Alignment _leftOnly;
		private static readonly Alignment _rightOnly;
		private static readonly Alignment _topOnly;
		private static readonly Alignment _bottomOnly;
		private static readonly Alignment _center;
		private static readonly Alignment _horizontalCenter;
		private static readonly Alignment _verticalCenter;
		private static readonly Alignment _topLeft;
		private static readonly Alignment _topRight;
		private static readonly Alignment _bottomLeft;
		private static readonly Alignment _bottomRight;
		private static readonly Alignment _leftCenter;
		private static readonly Alignment _rightCenter;
		private static readonly Alignment _topCenter;
		private static readonly Alignment _bottomCenter;
		private readonly Func<Rectangle, Rectangle, Rectangle> _adjustment;

		static Alignment()
		{
			_none = new Alignment((p, c) => c);

			_leftOnly = new Alignment((p, c) => new Rectangle(p.X, c.Y, c.Width, c.Height));
			_rightOnly = new Alignment((p, c) => new Rectangle(p.Right - c.Width, c.Y, c.Width, c.Height));
			_topOnly = new Alignment((p, c) => new Rectangle(c.X, p.Y, c.Width, c.Height));
			_bottomOnly = new Alignment((p, c) => new Rectangle(c.X, p.Bottom - c.Height, c.Width, c.Height));

			_topLeft = new Alignment((p, c) => _topOnly.Adjust(p, _leftOnly.Adjust(p, c)));
			_topRight = new Alignment((p, c) => _topOnly.Adjust(p, _rightOnly.Adjust(p, c)));
			_bottomLeft = new Alignment((p, c) => _bottomOnly.Adjust(p, _leftOnly.Adjust(p, c)));
			_bottomRight = new Alignment((p, c) => _bottomOnly.Adjust(p, _rightOnly.Adjust(p, c)));

			_center = new Alignment((p, c) => new Rectangle(p.X + (p.Width - c.Width) / 2, p.Y + (p.Height - c.Height) / 2, c.Width, c.Height));
			_horizontalCenter = new Alignment((p, c) => new Rectangle(p.X + (p.Width - c.Width) / 2, c.Y, c.Width, c.Height));
			_verticalCenter = new Alignment((p, c) => new Rectangle(c.X, p.Y + (p.Height - c.Height) / 2, c.Width, c.Height));

			_leftCenter = new Alignment((p, c) => _leftOnly.Adjust(p, _verticalCenter.Adjust(p, c)));
			_rightCenter = new Alignment((p, c) => _rightOnly.Adjust(p, _verticalCenter.Adjust(p, c)));
			_topCenter = new Alignment((p, c) => _topOnly.Adjust(p, _horizontalCenter.Adjust(p, c)));
			_bottomCenter = new Alignment((p, c) => _bottomOnly.Adjust(p, _horizontalCenter.Adjust(p, c)));
		}

		private Alignment(Func<Rectangle, Rectangle, Rectangle> adjustment)
		{
			_adjustment = adjustment;
		}

		public static Alignment None
		{
			get { return _none; }
		}

		public static Alignment LeftOnly
		{
			get { return _leftOnly; }
		}

		public static Alignment RightOnly
		{
			get { return _rightOnly; }
		}

		public static Alignment TopOnly
		{
			get { return _topOnly; }
		}

		public static Alignment BottomOnly
		{
			get { return _bottomOnly; }
		}

		public static Alignment TopLeft
		{
			get { return _topLeft; }
		}

		public static Alignment TopRight
		{
			get { return _topRight; }
		}

		public static Alignment BottomLeft
		{
			get { return _bottomLeft; }
		}

		public static Alignment BottomRight
		{
			get { return _bottomRight; }
		}

		public static Alignment HorizontalCenter
		{
			get { return _horizontalCenter; }
		}

		public static Alignment VerticalCenter
		{
			get { return _verticalCenter; }
		}

		public static Alignment Center
		{
			get { return _center; }
		}

		public static Alignment LeftCenter
		{
			get { return _leftCenter; }
		}

		public static Alignment RightCenter
		{
			get { return _rightCenter; }
		}

		public static Alignment TopCenter
		{
			get { return _topCenter; }
		}

		public static Alignment BottomCenter
		{
			get { return _bottomCenter; }
		}

		public Rectangle Adjust(Rectangle parent, Rectangle child)
		{
			return _adjustment(parent, child);
		}
	}
}
