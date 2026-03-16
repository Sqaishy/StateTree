using System;
using UnityEngine;

namespace StateTree
{
	/// <summary>
	/// Chooses a branch depending on the condition result
	/// </summary>
	[Serializable]
	public class Branch : Flow
	{
		[SerializeReference] private StateNode trueNode;
		[SerializeReference] private StateNode falseNode;

		private Condition[] conditions;
		private ConditionOperator conditionOperator;
		private StateNode currentChild;

		public StateNode TrueNode
		{
			get => trueNode;
			set
			{
				if (trueNode != null)
					RemoveChild(trueNode);

				trueNode = value;
				AddChild(trueNode);
			}
		}

		public StateNode FalseNode
		{
			get => falseNode;
			set
			{
				if (falseNode != null)
					RemoveChild(falseNode);

				falseNode = value;
				AddChild(falseNode);
			}
		}

		public Branch(Condition[] conditions) : this(conditions, ConditionOperator.AllTrue)
		{

		}

		public Branch(Condition[] conditions, ConditionOperator conditionOperator)
		{
			this.conditions = conditions;
			this.conditionOperator = conditionOperator;
		}

		protected override Status Enter()
		{
			foreach (Condition condition in conditions)
			{
				condition.SetAgent(Module.Graph.Agent);
				condition.Enter();
			}

			Status status;

			if (Condition.CheckConditions(conditions, conditionOperator))
			{
				//Start the True node
				status = Module.StartNode(trueNode);
				currentChild = trueNode;
			}
			else
			{
				//Start the false node
				status = Module.StartNode(falseNode);
				currentChild = falseNode;
			}

			return status;
		}

		protected override Status Update()
		{
			Status status = currentChild.OnUpdate();

			if (status is Status.Success or Status.Failure)
				return status;

			if (Condition.CheckConditions(conditions, conditionOperator))
			{
				if (currentChild != TrueNode)
					SwitchBranch(TrueNode);
			}
			else
			{
				if (currentChild != FalseNode)
					SwitchBranch(FalseNode);
			}

			return status;
		}

		protected override void Exit()
		{
			currentChild = null;

			foreach (Condition condition in conditions)
				condition.Exit();
		}

		private void SwitchBranch(StateNode newChild)
		{
			Module.ExitNode(currentChild);
			currentChild = newChild;
			Module.StartNode(currentChild);
		}

		protected override Status StartChild(Status childStatus)
		{
			return childStatus;
		}
	}
}