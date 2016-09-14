using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xu.Graphics.UI.Layout;

namespace Xu.Graphics.UI.Controls
{
	public abstract class Control
	{
		private Bounds _bounds;
		private Rectangle _viewportRectangle;
		private bool _viewportRectangleDirty;

		protected Control()
		{
			Controls = new HashSet<Control>();
			_viewportRectangleDirty = true;
		}

		protected internal HashSet<Control> Controls { get; private set; }

		public virtual Rectangle ViewportRectangle
		{
			get
			{
				if (_viewportRectangleDirty)
				{
					_viewportRectangle = CalculateViewportRectangle();
					_viewportRectangleDirty = false;
				}

				return _viewportRectangle;
			}
		}

		public bool ContainsCursor { get; private set; }
		public bool HasFocus { get; private set; }

		public Bounds Bounds
		{
			get { return _bounds; }
			set
			{
				_bounds = value;
				SetViewportRectangleDirty();
			}
		}

		public Control Parent { get; private set; }

		private void SetViewportRectangleDirty()
		{
			_viewportRectangleDirty = true;

			foreach (Control control in Controls)
			{
				control.SetViewportRectangleDirty();
			}
		}

		public bool AddControl(Control control)
		{
			if (control.Parent != null)
			{
				throw new InvalidOperationException("Can't AddControl that already has a Parent");
			}

			control.Parent = this;
			return Controls.Add(control);
		}

		public bool RemoveControl(Control control)
		{
			if (control.Parent != this)
			{
				throw new InvalidOperationException("Can't RemoveControl as this control isn't its Parent");
			}

			control.Parent = null;
			return Controls.Remove(control);
		}

		private Rectangle CalculateViewportRectangle()
		{
			Rectangle result;

			Rectangle parentRectangle = Parent.ViewportRectangle;

			int left = parentRectangle.Left + (int) (parentRectangle.Width * Bounds.Left.Fraction) + Bounds.Left.Value;
			int right = parentRectangle.Left + (int) (parentRectangle.Width * Bounds.Right.Fraction) + Bounds.Right.Value;
			int top = parentRectangle.Top + (int) (parentRectangle.Height * Bounds.Top.Fraction) + Bounds.Top.Value;
			int bottom = parentRectangle.Top + (int) (parentRectangle.Height * Bounds.Bottom.Fraction) + Bounds.Bottom.Value;

			result.X = left;
			result.Y = top;
			result.Width = right - left;
			result.Height = bottom - top;

			return result;
		}

		public abstract void Draw(IControlVisualizer visualizer, GameTime gameTime);

		public virtual void DoCursorEnter()
		{
			ContainsCursor = true;
			OnCursorEntered(new UIEventArgs());
		}

		public virtual void DoCursorLeave()
		{
			ContainsCursor = false;
			OnCursorLeft(new UIEventArgs());
		}

		public virtual void DoLeftClick()
		{
			OnLeftClicked(new UIEventArgs());
		}

		public virtual void DoFocusGet()
		{
			HasFocus = true;
			OnFocusGot(new UIEventArgs());
		}

		public virtual void DoFocusLose()
		{
			HasFocus = false;
			OnFocusLost(new UIEventArgs());
		}

		public event UIEvent CursorEntered;
		public event UIEvent CursorLeft;
		public event UIEvent LeftClicked;
		public event UIEvent FocusGot;
		public event UIEvent FocusLost;

		private void OnCursorLeft(UIEventArgs args)
		{
			UIEvent handler = CursorLeft;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void OnLeftClicked(UIEventArgs args)
		{
			UIEvent handler = LeftClicked;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void OnFocusGot(UIEventArgs args)
		{
			UIEvent handler = FocusGot;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void OnFocusLost(UIEventArgs args)
		{
			UIEvent handler = FocusLost;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void OnCursorEntered(UIEventArgs args)
		{
			UIEvent handler = CursorEntered;
			if (handler != null)
			{
				handler(this, args);
			}
		}
	}
}
