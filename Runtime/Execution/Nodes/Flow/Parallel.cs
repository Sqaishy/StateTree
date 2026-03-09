using System;

namespace StateTree
{
	/// <summary>
	/// Executes all children at the same time until all succeed or fail
	/// </summary>
	[Serializable]
	public class Parallel : Flow
	{
		protected override Status Enter()
		{
			int completedCount = 0;

			foreach (StateNode child in Children)
			{
				Status status = StartChild(Module.StartNode(child));

				if (status is Status.Success or Status.Failure)
					completedCount++;
			}

			return completedCount == Children.Count ? Status.Success : Status.Running;
		}

		protected override Status Update()
		{
			int completedCount = 0;

			foreach (StateNode child in Children)
			{
				Status status = child.OnUpdate();

				if (status is Status.Success or Status.Failure)
					completedCount++;
			}

			return completedCount == Children.Count ? Status.Success : Status.Running;
		}

		protected override void Exit()
		{
			foreach (StateNode child in Children)
				Module.ExitNode(child);
		}

		protected override Status StartChild(Status childStatus)
		{
			return childStatus;
		}
	}
}