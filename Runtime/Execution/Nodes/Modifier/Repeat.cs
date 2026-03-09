using System.Collections.Generic;

namespace StateTree
{
	/// <summary>
	/// Repeats operation of the child node
	/// </summary>
	public class Repeat : Modifier
	{
		protected override Status Enter()
		{
			Module.StartNode(Child);

			return Status.Running;
		}

		protected override Status Update()
		{
			Status status = Child.OnUpdate();

			if (status is Status.Failure or Status.Success)
				RestartChild();

			return Status.Running;
		}

		protected override void Exit()
		{
			Module.ExitNode(Child);
		}
	}
}