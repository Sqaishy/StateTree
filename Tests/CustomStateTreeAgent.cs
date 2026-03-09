using System;
using Unity.Behavior;
using UnityEngine;

namespace StateTree
{
	public class CustomStateTreeAgent : StateTreeAgent
	{
		protected override void Awake()
		{
			Blackboard.TryGetContext(out CharacterContext context);

			StateModule defaultModule = new StateModule("DefaultState");
			defaultModule.SetRoot(new StatusAction(Status.Running));

			StateModule runModule = new StateModule("RunState");
			runModule.SetRoot(new WaitAction(5f));

			Condition predicateCondition = new PredicateCondition(() => context.isTrue);
			defaultModule.AddTransition(runModule, new [] { predicateCondition},
				ConditionOperator.AnyTrue);

			StateGraph = new StateGraphBuilder(this, defaultModule).Build();

			base.Awake();
		}

		protected override void CreateBlackboard()
		{
			Blackboard = new StateBlackboard()
			{
				Contexts =
				{
					new CharacterContext(),
					new TestContext(),
					new CustomStateContext(),
				}
			};
		}
	}

	[Serializable]
	public class CustomStateContext : StateContext
	{
		public float customFloat;

		public override StateContext Clone()
		{
			return new CustomStateContext()
			{
				customFloat = customFloat,
			};
		}
	}
}