using System;

namespace Xu.Input
{
	public class MouseEventMapping
	{
		public Action<int, int> MoveRelativeAction { get; set; }
		public Action LeftDownAction { get; set; }
		public Action LeftUpAction { get; set; }
		public Action RightDownAction { get; set; }
		public Action RightUpAction { get; set; }
	}
}