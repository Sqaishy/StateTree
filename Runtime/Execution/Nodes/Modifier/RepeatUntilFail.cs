namespace StateTree
{
	/// <summary>
	/// Repeats operation of the child node until failure
	/// </summary>
	public class RepeatUntilFail : Modifier
	{
		protected override Status Enter()
		{
			Status status = Module.StartNode(Child);

			return status switch
			{
				Status.Failure => Status.Success,
				_ => Status.Running
			};
		}

		protected override Status Update()
		{
			Status status = Child.OnUpdate();

			switch (status)
			{
				case Status.Success:
					RestartChild();
					break;
				case Status.Failure:
					return Status.Success;
			}

			return Status.Running;
		}
	}
}