namespace StateTree
{
	public class Transition
	{
		public readonly StateModule from;
		public readonly StateModule to;
		public readonly Condition[] conditions;
		public readonly ConditionOperator conditionOperator;

		public Transition(StateModule from, StateModule to, Condition[] conditions,
			ConditionOperator conditionOperator)
		{
			(this.from, this.to, this.conditions, this.conditionOperator) =
				(from, to, conditions, conditionOperator);
		}

		internal void EnterTransition()
		{
			foreach (Condition condition in conditions)
				condition.Enter();
		}

		internal void ExitTransition()
		{
			foreach (Condition condition in conditions)
				condition.Exit();
		}

		internal bool CanTransition()
		{
			//Check the array of conditions to see if any succeed
			return Condition.CheckConditions(conditions, conditionOperator);
		}

		internal void SetConditionAgent(StateTreeAgent agent)
		{
			foreach (Condition condition in conditions)
				condition.SetAgent(agent);
		}
	}
}