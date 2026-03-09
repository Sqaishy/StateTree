using System;

namespace StateTree
{
	/// <summary>
	/// Invoke the provided system.Action and return success
	/// </summary>
	public class InvokeAction : ActionNode
	{
		private readonly Action action;

		public InvokeAction(Action action)
		{
			this.action = action;
		}

		protected override Status Enter()
		{
			return Status.Running;
		}

		protected override Status Update()
		{
			action();
			return Status.Success;
		}
	}
}