using System.Collections.Generic;

namespace StateTree
{
	public abstract class Condition
	{
		/// <summary>
		/// When true returns the opposite result for the condition
		/// </summary>
		public bool Negate { get; set; }
		protected StateTreeAgent Agent { get; private set; }

		public abstract bool IsTrue();

		internal void SetAgent(StateTreeAgent agent) => Agent = agent;

		public virtual void Enter() { }
		public virtual void Exit() { }

		private bool CheckCondition()
		{
			bool result = IsTrue();
			return Negate ? !result : result;
		}

		public static bool CheckConditions(Condition[] conditions, ConditionOperator conditionOperator)
		{
			if (conditionOperator == ConditionOperator.AnyTrue)
			{
				foreach (Condition condition in conditions)
				{
					if (condition.CheckCondition())
						return true;
				}

				return false;
			}

			foreach (Condition condition in conditions)
			{
				if (!condition.CheckCondition())
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