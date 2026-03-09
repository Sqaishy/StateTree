using System;

namespace StateTree
{
	public class PredicateCondition : Condition
	{
		private readonly Func<bool> predicate;

		public PredicateCondition(Func<bool> predicate)
		{
			this.predicate = predicate;
		}

		public override bool IsTrue()
		{
			return predicate();
		}
	}
}