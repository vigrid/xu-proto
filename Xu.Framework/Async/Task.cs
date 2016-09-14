using System;

namespace Xu.Async
{
	public class Task
	{
		private readonly Action _action;
		private readonly Func<float> _priorityFunc;

		internal Task(Action action, Func<float> priorityFunc)
		{
			_action = action;
			_priorityFunc = priorityFunc;

			EvaluatePriority();
		}

		public float Priority { get; private set; }

		public Action Action
		{
			get { return _action; }
		}

		public void EvaluatePriority()
		{
			Priority = _priorityFunc();
		}
	}
}
