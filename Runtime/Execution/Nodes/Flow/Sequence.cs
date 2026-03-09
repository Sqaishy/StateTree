
using System;

namespace StateTree
{
	/// <summary>
	/// Executes children in order until one fails or all succeed
	/// </summary>
	[NodeSearch(
		name: null,
		description: "Executes children in order until one fails or all succeed",
		category: "Flow")]
	[Serializable]
	public class Sequence : Flow
	{
		protected override Status Enter()
		{
			return StartChild(Module.StartNode(Children[currentChildIndex]));
		}

		protected override Status Update()
		{
			Status childStatus = Children[currentChildIndex].OnUpdate();

			if (childStatus != Status.Success)
				return childStatus;

			if (currentChildIndex >= Children.Count - 1)
				return Status.Success;

			return IncrementChild();
		}

		protected override void Exit()
		{
			Module.ExitNode(Children[currentChildIndex]);

			currentChildIndex = 0;
		}

		protected override Status StartChild(Status childStatus) =>
			childStatus switch
			{
				Status.Success => currentChildIndex >= Children.Count - 1 ? Status.Success : IncrementChild(),
				_ => childStatus
			};
	}
}