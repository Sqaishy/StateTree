namespace StateTree
{
	/// <summary>
	/// Fails the child if the given conditions are met
	/// </summary>
	public class FailIf : Modifier
	{
		private Condition[] conditions;
		private ConditionOperator conditionOperator;

		public FailIf(Condition[] conditions) : this(conditions, ConditionOperator.AllTrue)
		{

		}

		public FailIf(Condition[] conditions, ConditionOperator conditionOperator)
		{
			this.conditions = conditions;
			this.conditionOperator = conditionOperator;
		}

		protected override Status Enter()
		{
			foreach (Condition condition in conditions)
			{
				//TODO Move this stupid shit out of the enter and into when the graph is being made
				//Also same dumb shit in ContinueIf class
				condition.SetAgent(Module.Graph.Agent);
				condition.Enter();
			}

			if (Condition.CheckConditions(conditions, conditionOperator))
				return Status.Failure;

			return Module.StartNode(Child) switch
			{
				Status.Success => Status.Success,
				Status.Failure => Status.Failure,
				_ => Status.Running
			};
		}

		protected override Status Update()
		{
			Status status = Child.OnUpdate();

			if (Condition.CheckConditions(conditions, conditionOperator))
				return Status.Failure;

			return status;
		}

		protected override void Exit()
		{
			foreach (Condition condition in conditions)
				condition.Exit();

			base.Exit();
		}
	}
}