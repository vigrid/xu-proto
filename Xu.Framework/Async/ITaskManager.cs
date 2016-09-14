using System;

namespace Xu.Async
{
	public interface ITaskManager
	{
		Task Schedule(Action action, Func<float> dynamicPriority);
		Task Schedule(Action action, float priority);
	}
}
