using System;

namespace StateTree
{
	/// <summary>
	/// Executes all children at the same time stopping if one succeeds or fails
	/// </summary>
	[Serializable]
	public class ParallelUntil : Flow
	{
		protected override Status Enter()
		{
			foreach (StateNode child in Children)
			{
				Status status = StartChild(Module.StartNode(child));

				if (status is Status.Success or Status.Failure)
					return status;
			}

			return Status.Running;
		}

		protected override Status Update()
		{
			foreach (StateNode child in Children)
			{
				Status status = child.OnUpdate();

				if (status is Status.Success or Status.Failure)
					return status;
			}

			return Status.Running;
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