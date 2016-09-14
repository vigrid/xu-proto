using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public class KeyboardManager : GameComponent, IKeyboardManager
	{
		private readonly Dictionary<Keys, Action> _onHold = new Dictionary<Keys, Action>();
		private readonly Dictionary<Keys, Action> _onPress = new Dictionary<Keys, Action>();

		private KeyboardBuffer _buffer;
		private KeyboardState _state;

		public KeyboardManager(Game game) : base(game)
		{
			Contract.Requires(game != null);
			Contract.EndContractBlock();
		}

		#region IKeyboardManager Members

		public void BindHold(Keys key, Action action)
		{
			Contract.Requires(action != null);
			Contract.EndContractBlock();

			_onHold.Add(key, action);
		}

		public void BindPress(Keys key, Action action)
		{
			Contract.Requires(action != null);
			Contract.EndContractBlock();

			_onPress.Add(key, action);
		}

		public void UnbindHold(Keys key)
		{
			_onHold.Remove(key);
		}

		public void UnbindPress(Keys key)
		{
			_onPress.Remove(key);
		}

		private KeyboardManagerMode _mode;
		public KeyboardManagerMode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode != value)
				{
					if (value == KeyboardManagerMode.Buffered)
					{
						_buffer.Clear();
						_buffer.Enabled = true;
					}
					else
					{
						_buffer.Enabled = false;
						_buffer.Clear();
					}
					_mode = value;
				}
			}
		}

		public IKeyConsumer BufferedKeyConsumer { get; set; }

		#endregion

		public override void Initialize()
		{
			_state = Keyboard.GetState();
			_buffer = new KeyboardBuffer(Game.Window.Handle);

			base.Initialize();
		}

		protected override void OnEnabledChanged(object sender, EventArgs args)
		{
			throw new NotSupportedException("KeyboardManager does not support runtime Enable/Disable.");
		}

		public override void Update(GameTime gameTime)
		{
			KeyboardState currentState = Keyboard.GetState();

			if (_mode == KeyboardManagerMode.Direct)
			{
				ProcessBindings(currentState);
			}
			else
			{
				ProcessBuffer();
			}

			_state = currentState;

			base.Update(gameTime);
		}

		private void ProcessBuffer()
		{
			while (_buffer.HasData)
			{
				KeyboardBufferItem item = _buffer.Dequeue();
				if (BufferedKeyConsumer != null)
				{
					BufferedKeyConsumer.Consume(item);
				}
			}
		}

		private void ProcessBindings(KeyboardState currentState)
		{
			foreach (KeyValuePair<Keys, Action> binding in _onHold)
			{
				if (currentState.IsKeyDown(binding.Key))
				{
					binding.Value();
				}
			}

			foreach (KeyValuePair<Keys, Action> binding in _onPress)
			{
				if (currentState.IsKeyDown(binding.Key) && _state.IsKeyUp(binding.Key))
				{
					binding.Value();
				}
			}
		}
	}
}
