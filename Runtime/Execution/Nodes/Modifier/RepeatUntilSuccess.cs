namespace StateTree
{
	/// <summary>
	/// Repeats operation of the child node until success
	/// </summary>
	public class RepeatUntilSuccess : Modifier
	{
		protected override Status Enter()
		{
			Status status = Module.StartNode(Child);

			return status switch
			{
				Status.Success => Status.Success,
				_ => Status.Running
			};
		}

		protected override Status Update()
		{
			Status status = Child.OnUpdate();

			switch (status)
			{
				case Status.Failure:
					RestartChild();
					break;
				case Status.Success:
					return Status.Success;
			}

			return Status.Running;
		}
	}
}