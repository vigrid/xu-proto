using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public class MouseManager : GameComponent, IMouseManager
	{
		private readonly Dictionary<MouseManagerMode, MouseEventMapping> _eventMappings;

		private MouseState _state;

		public MouseManager(Game game) : base(game)
		{
			Contract.Requires(game != null);
			Contract.EndContractBlock();

			_eventMappings = new Dictionary<MouseManagerMode, MouseEventMapping>();
			_eventMappings[MouseManagerMode.Direct] = new MouseEventMapping();
			_eventMappings[MouseManagerMode.UI] = new MouseEventMapping();
		}

		#region IMouseManager Members

		public MouseManagerMode Mode { get; set; }

		public MouseState State
		{
			get { return _state; }
		}

		public void BindLeftButton(MouseManagerMode mode, Action downAction, Action upAction)
		{
			Contract.Requires(downAction != null);
			Contract.Requires(upAction != null);
			Contract.EndContractBlock();

			_eventMappings[mode].LeftDownAction = downAction;
			_eventMappings[mode].LeftUpAction = upAction;
		}

		public void BindRightButton(MouseManagerMode mode, Action downAction, Action upAction)
		{
			Contract.Requires(downAction != null);
			Contract.Requires(upAction != null);
			Contract.EndContractBlock();

			_eventMappings[mode].RightDownAction = downAction;
			_eventMappings[mode].RightUpAction = upAction;
		}

		public void BindMoveRelative(MouseManagerMode mode, Action<int, int> action)
		{
			Contract.Requires(action != null);
			Contract.EndContractBlock();

			_eventMappings[mode].MoveRelativeAction = action;
		}

		public void RaiseLeftButtonDown(MouseManagerMode mode)
		{
			TryRaise(_eventMappings[mode].LeftDownAction);
		}

		public void RaiseRightButtonDown(MouseManagerMode mode)
		{
			TryRaise(_eventMappings[mode].RightDownAction);
		}

		#endregion

		public override void Initialize()
		{
			_state = Mouse.GetState();

			base.Initialize();
		}

		private void TryRaise(Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public override void Update(GameTime gameTime)
		{
			Game.IsMouseVisible = Mode == MouseManagerMode.UI;

			MouseState newState = Mouse.GetState();

			if (newState.LeftButton == ButtonState.Pressed && _state.LeftButton == ButtonState.Released)
			{
				TryRaise(_eventMappings[Mode].LeftDownAction);
			}
			if (newState.RightButton == ButtonState.Pressed && _state.RightButton == ButtonState.Released)
			{
				TryRaise(_eventMappings[Mode].RightDownAction);
			}

			if (newState.LeftButton == ButtonState.Released && _state.LeftButton == ButtonState.Pressed)
			{
				TryRaise(_eventMappings[Mode].LeftUpAction);
			}
			if (newState.RightButton == ButtonState.Released && _state.RightButton == ButtonState.Pressed)
			{
				TryRaise(_eventMappings[Mode].RightUpAction);
			}

			if (Mode == MouseManagerMode.Direct)
			{
				int cx = Game.Window.ClientBounds.Width / 2;
				int cy = Game.Window.ClientBounds.Height / 2;

				int dx = newState.X - cx;
				int dy = newState.Y - cy;

				Mouse.SetPosition(cx, cy);

				_eventMappings[Mode].MoveRelativeAction(dx, dy);
			}

			_state = newState;

			base.Update(gameTime);
		}
	}
}
