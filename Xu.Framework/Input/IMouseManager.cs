using System;
using Microsoft.Xna.Framework.Input;

namespace Xu.Input
{
	public interface IMouseManager
	{
		MouseManagerMode Mode { get; set; }
		MouseState State { get; }

		void BindLeftButton(MouseManagerMode mode, Action downAction, Action upAction);
		void BindRightButton(MouseManagerMode mode, Action downAction, Action upAction);
		void BindMoveRelative(MouseManagerMode mode, Action<int, int> action);

		void RaiseLeftButtonDown(MouseManagerMode mode);
		void RaiseRightButtonDown(MouseManagerMode mode);
	}
}
