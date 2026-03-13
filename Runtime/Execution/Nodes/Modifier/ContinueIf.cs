namespace StateTree
{
	/// <summary>
	/// Continues the branch if the given conditions are met
	/// </summary>
	public class ContinueIf : Modifier
	{
		private Condition[] conditions;
		private ConditionOperator conditionOperator;
		private bool childStarted;

		public ContinueIf(Condition[] conditions) : this(conditions, ConditionOperator.AllTrue)
		{

		}

		public ContinueIf(Condition[] conditions, ConditionOperator conditionOperator)
		{
			this.conditions = conditions;
			this.conditionOperator = conditionOperator;

			foreach (Condition condition in this.conditions)
				condition.SetAgent(Module.Graph.Agent);
		}

		protected override Status Enter()
		{
			foreach (Condition condition in conditions)
				condition.Enter();

			if (!Condition.CheckConditions(conditions, conditionOperator))
				return Status.Running;

			return StartChild();
		}

		protected override Status Update()
		{
			if (!Condition.CheckConditions(conditions, conditionOperator))
				return Status.Running;

			if (!childStarted)
				return StartChild();

			return Child.OnUpdate();
		}

		protected override void Exit()
		{
			childStarted = false;

			foreach (Condition condition in conditions)
				condition.Exit();

			base.Exit();
		}

		private Status StartChild()
		{
			childStarted = true;
			return Module.StartNode(Child);
		}
	}
}