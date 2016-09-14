using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using Microsoft.Xna.Framework;


namespace Xu.Async
{
	public class TaskManager : GameComponent, ITaskManager
	{
		private readonly int _workerThreads;
		private List<Task> _tasks;
		private Thread[] _threads;

		public TaskManager(Game game) : base(game)
		{
			Contract.Requires(game != null);
			Contract.EndContractBlock();

			_workerThreads = 0;
		}

		public TaskManager(Game game, int workerThreads) : base(game)
		{
			Contract.Requires(game != null);
			Contract.Requires(workerThreads > 0);
			Contract.EndContractBlock();

			_workerThreads = workerThreads;
		}

		#region ITaskManager Members

		public Task Schedule(Action action, Func<float> dynamicPriority)
		{
			Contract.Requires(dynamicPriority != null);
			Contract.EndContractBlock();

			lock (_tasks)
			{
				var task = new Task(action, dynamicPriority);
				_tasks.Add(task);
				Monitor.Pulse(_tasks);
				return task;
			}
		}

		#endregion

		public Task Schedule(Action action, float priority)
		{
			Contract.Requires(action != null);
			Contract.EndContractBlock();

			return Schedule(action, () => priority);
		}

		public override void Initialize()
		{
			// int threads = _workerThreads != 0 ? _workerThreads : Math.Max(1, Environment.ProcessorCount - 1);
            int threads = 1;

			_tasks = new List<Task>();
			_threads = new Thread[threads];

			for (int i = 0; i < threads; i++)
			{
				_threads[i] = new Thread(WorkerLoop);
				_threads[i].Start();
			}

			// Although it does nothing, call it for consistency
			base.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			lock (_tasks)
			{
				foreach (Task task in _tasks)
				{
					task.EvaluatePriority();
				}
			}

			// Although it does nothing, call it for consistency
			base.Update(gameTime);
		}

		protected override void OnEnabledChanged(object sender, EventArgs args)
		{
			throw new NotSupportedException("TaskManager does not support runtime Enable/Disable.");
		}

		protected override void Dispose(bool disposing)
		{
			if (_threads != null)
			{
				foreach (Thread thread in _threads)
				{
					Schedule(null, () => 0.0f);
				}
			}

			base.Dispose(disposing);
		}

		private Task DequeueTask()
		{
			Task taskToDequeue = null;

			_tasks.RemoveAll(task => task.Priority < 0.0f);

			float bestPriority = Single.MaxValue;

			for (int i = 0; i < _tasks.Count; i++)
			{
				Task task = _tasks[i];
				if (task.Action == null)
				{
					_tasks.RemoveAt(i);
					return task;
				}
				float taskPriority = task.Priority;
				if (taskPriority < bestPriority)
				{
					taskToDequeue = task;
					bestPriority = taskPriority;
				}
			}

			_tasks.Remove(taskToDequeue);

			return taskToDequeue;
		}

		private void WorkerLoop()
		{
			while (true)
			{
				Task task;

				lock (_tasks)
				{
					while (_tasks.Count == 0)
					{
						Monitor.Wait(_tasks);
					}

					task = DequeueTask();
					if (task == null)
					{
						break;
					}
				}

				if (task.Action == null)
				{
					return;
				}

				task.Action();
			}
		}
	}
}
