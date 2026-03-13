using System.Collections.Generic;

namespace StateTree
{
	public abstract class Condition
	{
		protected StateTreeAgent Agent { get; private set; }

		public abstract bool IsTrue();

		internal void SetAgent(StateTreeAgent agent) => Agent = agent;

		public virtual void Enter() { }
		public virtual void Exit() { }

		public static bool CheckConditions(Condition[] conditions, ConditionOperator conditionOperator)
		{
			if (conditionOperator == ConditionOperator.AnyTrue)
			{
				foreach (Condition condition in conditions)
				{
					if (condition.IsTrue())
						return true;
				}

				return false;
			}

			foreach (Condition condition in conditions)
			{
				if (!condition.IsTrue())
					return false;
			}

			return true;
		}
	}

	public enum ConditionOperator
	{
		AnyTrue,
		AllTrue,
	}
}