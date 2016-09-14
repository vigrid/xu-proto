using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Xu.Windows;

namespace Xu.Input
{
	public class KeyboardBuffer : Hook<Win32.Win32MsgStruct>
	{
		private enum Win32ModifierVKeys
		{
			Shift = 0x10,
			Control = 0x11,
			Alt = 0x12,
		}

		private static readonly Keys[] ControlKeys = new[]
		{
			Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.Enter, Keys.Back, Keys.Delete, Keys.PageUp, Keys.PageDown, Keys.Escape, Keys.Tab, 
		};

		private static readonly Win32ModifierVKeys[] ModifierKeys = new[]
		{
			Win32ModifierVKeys.Shift, Win32ModifierVKeys.Control, Win32ModifierVKeys.Alt
		};

		private static readonly Dictionary<Win32ModifierVKeys, KeyboardBufferItem.Modifiers> ModifierEffect = new Dictionary<Win32ModifierVKeys, KeyboardBufferItem.Modifiers>
		{
			{ Win32ModifierVKeys.Shift, KeyboardBufferItem.Modifiers.Shift },
			{ Win32ModifierVKeys.Control, KeyboardBufferItem.Modifiers.Control },
			{ Win32ModifierVKeys.Alt, KeyboardBufferItem.Modifiers.Alt },
		};

		private readonly Queue<KeyboardBufferItem> _queue;
		private readonly Dictionary<Win32ModifierVKeys, bool> _modifierState;

		private readonly char[] _charBuffer = new char[8];
		private readonly byte[] _byteBuffer = new byte[8];

		public KeyboardBuffer(IntPtr windowHandle) : base(windowHandle, Win32.WindowsHookType.GetMessage)
		{
			_queue = new Queue<KeyboardBufferItem>();
			_modifierState = ModifierKeys.ToDictionary(mk => mk, mk => false);
		}

		public bool Enabled { get; set; }

		public bool HasData
		{
			get { return _queue.Count > 0; }
		}

		public void Clear()
		{
			_queue.Clear();
		}

		public KeyboardBufferItem Dequeue()
		{
			return _queue.Dequeue();
		}

		protected override void HandleMessage(ref Win32.Win32MsgStruct data)
		{
			if (Enabled)
			{
				switch (data.Message)
				{
					case Win32.Win32WM.CHAR:
					{
						var modifiers = GetCurrentModifiers();
						if ((modifiers & (KeyboardBufferItem.Modifiers.Control | KeyboardBufferItem.Modifiers.Alt)) != KeyboardBufferItem.Modifiers.Control)
						{
							EnqueueChar((char) data.WParam);
						}
						break;
					}
					case Win32.Win32WM.KEYDOWN:
					{
						var xnaVirtualKey = (Keys) data.WParam;
						var win32VirtualKey = (Win32ModifierVKeys) data.WParam;
						if (ControlKeys.Contains(xnaVirtualKey))
						{
							EnqueueVirtualKey(xnaVirtualKey);
						}
						else if (ModifierKeys.Contains(win32VirtualKey) && win32VirtualKey != Win32ModifierVKeys.Control)
						{
							_modifierState[win32VirtualKey] = true;
						}
						else
						{
							Win32.TranslateMessage(ref data);
						}
						break;
					}
					case Win32.Win32WM.KEYUP:
					{
						var win32VirtualKey = (Win32ModifierVKeys) data.WParam;
						if (ModifierKeys.Contains(win32VirtualKey) && win32VirtualKey != Win32ModifierVKeys.Control)
						{
							_modifierState[win32VirtualKey] = false;
						}
						break;
					}
				}
			}
		}

		private char ConvertChar(char input)
		{
			// TODO: This shouldn't be hacked in like that...
			_charBuffer[0] = input;
			int bytes = Encoding.Unicode.GetBytes(_charBuffer, 0, 1, _byteBuffer, 0);
			Encoding.Default.GetChars(_byteBuffer, 0, bytes, _charBuffer, 0);
			return _charBuffer[0];
		}

		private void EnqueueVirtualKey(Keys key)
		{
			_queue.Enqueue(new KeyboardBufferItem(key, GetCurrentModifiers()));
		}

		private void EnqueueChar(char character)
		{
			_queue.Enqueue(new KeyboardBufferItem(ConvertChar(character), GetCurrentModifiers()));
		}

		private KeyboardBufferItem.Modifiers GetCurrentModifiers()
		{
			return _modifierState.Where(kv => kv.Value).Aggregate((KeyboardBufferItem.Modifiers) 0, (current, kv) => current | ModifierEffect[kv.Key]);
		}
	}
}
