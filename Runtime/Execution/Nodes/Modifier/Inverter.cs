namespace StateTree
{
	/// <summary>
	/// Inverts the result of the child node
	/// </summary>
	public class Inverter : Modifier
	{
		protected override Status Enter()
		{
			return Module.StartNode(Child) switch
			{
				Status.Success => Status.Failure,
				Status.Failure => Status.Success,
				_ => Status.Running
			};
		}

		protected override Status Update()
		{
			return Child.OnUpdate() switch
			{
				Status.Success => Status.Failure,
				Status.Failure => Status.Success,
				_ => Status.Running
			};
		}
	}
}