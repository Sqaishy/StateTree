using System;

namespace StateTree
{
	/// <summary>
	/// Executes a random child node
	/// </summary>
	[Serializable]
	public class RandomSelection : Flow
	{
		protected override Status Enter()
		{
			currentChildIndex = UnityEngine.Random.Range(0, Children.Count);

			return StartChild(Module.StartNode(Children[currentChildIndex]));
		}

		protected override Status Update()
		{
			return Children[currentChildIndex].OnUpdate();
		}

		protected override void Exit()
		{
			Module.ExitNode(Children[currentChildIndex]);
		}

		protected override Status StartChild(Status childStatus)
		{
			return childStatus;
		}
	}
}