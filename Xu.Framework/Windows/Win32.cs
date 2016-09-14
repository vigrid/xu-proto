using System;
using System.Runtime.InteropServices;

namespace Xu.Windows
{
	public static class Win32
	{
		#region Delegates

		public delegate bool HookProc(int code, IntPtr wParam, IntPtr lParam);

		#endregion

		#region Win32WM enum

		public enum Win32WM
		{
			KEYDOWN = 0x0100,
			KEYUP = 0x0101,
			CHAR = 0x0102,
		}

		#endregion

		#region WindowsHookType enum

		public enum WindowsHookType
		{
			GetMessage = 3,
		}

		#endregion

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr window, IntPtr module);

		[DllImport("user32.dll")]
		public static extern bool UnhookWindowsHookEx(IntPtr hookFunc);

		[DllImport("user32.dll")]
		public static extern bool CallNextHookEx(IntPtr hookFunc, int code, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(WindowsHookType hookType, HookProc hookProc, IntPtr module, uint threadId);

		[DllImport("user32.dll")]
		public static extern void TranslateMessage(ref Win32MsgStruct message);

		#region Nested type: Win32MsgStruct

		[StructLayout(LayoutKind.Sequential)]
		public struct Win32MsgStruct
		{
			public readonly IntPtr HWnd;
			public readonly Win32WM Message;
			public readonly uint WParam;
			public readonly uint LParam;
			public readonly uint Time;
			public readonly Win32PointStruct Point;
		}

		#endregion

		#region Nested type: Win32PointStruct

		[StructLayout(LayoutKind.Sequential)]
		public struct Win32PointStruct
		{
			public readonly int X;
			public readonly int y;
		}

		#endregion
	}
}
