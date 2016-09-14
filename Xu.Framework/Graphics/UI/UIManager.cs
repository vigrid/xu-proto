using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xu.Graphics.UI.Controls;
using Xu.Input;

namespace Xu.Graphics.UI
{
	public class UIManager : DrawableGameComponent
	{
		private readonly IKeyboardManager _keyboardManager;
		private readonly IMouseManager _mouseManager;
		private IControlVisualizer _visualizer;

		private Control _focusControl;
		private Control _hoverControl;

		private Point _mouseCursorPosition;
		private bool _mouseLeftClick;
		private bool _mouseRightClick;

		public UIManager(Game game, IKeyboardManager keyboardManager, IMouseManager mouseManager) : base(game)
		{
			_keyboardManager = keyboardManager;
			_mouseManager = mouseManager;

			_mouseManager.BindLeftButton(MouseManagerMode.UI, () => _mouseLeftClick = true, null);
			_mouseManager.BindRightButton(MouseManagerMode.UI, () => _mouseRightClick = true, null);

			Root = new ViewportContainer(game);
		}

		public ViewportContainer Root { get; private set; }

		protected override void LoadContent()
		{
			_visualizer = new ControlVisualizer(Game);

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			MouseState state = _mouseManager.State;
			_mouseCursorPosition.X = state.X;
			_mouseCursorPosition.Y = state.Y;

			Control hoverControl = FindControl(Root.Controls, _mouseCursorPosition);

			DoHoverEvents(hoverControl);
			DoLeftClickEvents(hoverControl);
			DoRightClickEvents(hoverControl);

			if (_focusControl != null && !_focusControl.HasFocus)
			{
				_focusControl = null;
			}
			_keyboardManager.BufferedKeyConsumer = _focusControl as IKeyConsumer;
			_keyboardManager.Mode = _focusControl is IKeyConsumer ? KeyboardManagerMode.Buffered : KeyboardManagerMode.Direct;

			_mouseLeftClick = false;
			_mouseRightClick = false;

			base.Update(gameTime);
		}

		private void DoLeftClickEvents(Control hoverControl)
		{
			if (_mouseLeftClick)
			{
				if (_focusControl != hoverControl)
				{
					if (_focusControl != null)
					{
						_focusControl.DoFocusLose();
					}
					_focusControl = hoverControl;
					if (_focusControl != null)
					{
						_focusControl.DoFocusGet();
					}
				}
				if (_focusControl == null)
				{
					_mouseManager.RaiseLeftButtonDown(MouseManagerMode.Direct);
				}
			}
		}

		private void DoRightClickEvents(Control hoverControl)
		{
			if (_mouseRightClick)
			{
				if (_focusControl == null)
				{
					_mouseManager.RaiseRightButtonDown(MouseManagerMode.Direct);
				}
			}
		}

		private void DoHoverEvents(Control hoverControl)
		{
			if (hoverControl != _hoverControl)
			{
				if (_hoverControl != null)
				{
					_hoverControl.DoCursorLeave();
				}
				_hoverControl = hoverControl;
				if (hoverControl != null)
				{
					_hoverControl.DoCursorEnter();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			_visualizer.BeginBatch();
			DrawRecursive(Root.Controls, gameTime);
			_visualizer.EndBatch();

			base.Draw(gameTime);
		}

		private void DrawRecursive(IEnumerable<Control> controls, GameTime gameTime)
		{
			foreach (Control control in controls)
			{
				control.Draw(_visualizer, gameTime);
				DrawRecursive(control.Controls, gameTime);
			}
		}

		private Control FindControl(IEnumerable<Control> controls, Point point)
		{
			return controls.Where(control => control.ViewportRectangle.Contains(point)).Select(control => FindControl(control.Controls, point) ?? control).FirstOrDefault();
		}
	}
}
