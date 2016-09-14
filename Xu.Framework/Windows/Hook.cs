using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Xu.Windows
{
	public abstract class Hook<TLParam> : IDisposable where TLParam : struct
	{
		private Win32.HookProc _hookProc;
		private IntPtr _hookProcHandle;

		protected Hook(IntPtr windowHandle, Win32.WindowsHookType hookType)
		{
			uint threadId = Win32.GetWindowThreadProcessId(windowHandle, IntPtr.Zero);

			_hookProcHandle = Win32.SetWindowsHookEx(hookType, _hookProc = HandleMessage, IntPtr.Zero, threadId);
			if (_hookProcHandle == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		private bool HandleMessage(int code, IntPtr wparam, IntPtr lparam)
		{
			if (code > -1 && wparam.ToInt32() == 1) // PM_REMOVE
			{
				var data = (TLParam) Marshal.PtrToStructure(lparam, typeof (TLParam));
				HandleMessage(ref data);
			}

			return Win32.CallNextHookEx(_hookProcHandle, code, wparam, lparam);
		}

		protected abstract void HandleMessage(ref TLParam data);

		private void Dispose(bool disposing)
		{
			if (disposing && _hookProcHandle != IntPtr.Zero && _hookProc != null)
			{
				Win32.UnhookWindowsHookEx(_hookProcHandle);
				_hookProcHandle = IntPtr.Zero;
				_hookProc = null;
			}
		}
	}
}
