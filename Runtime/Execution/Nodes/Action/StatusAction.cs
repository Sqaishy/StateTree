namespace StateTree
{
	/// <summary>
	/// Returns the provided Status
	/// </summary>
	public class StatusAction : ActionNode
	{
		private Status statusToReturn;

		public StatusAction(Status statusToReturn)
		{
			this.statusToReturn = statusToReturn;
		}

		protected override Status Enter()
		{
			return statusToReturn;
		}

		protected override Status Update()
		{
			return statusToReturn;
		}
	}
}