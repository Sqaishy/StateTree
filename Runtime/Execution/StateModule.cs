using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
	[Serializable]
	public class StateModule : IParent
	{
		public StateGraph Graph
		{
			get => graph;
			internal set => graph = value;
		}

		public string Name
		{
			get => name;
			set => name = value;
		}

		public List<Transition> Transitions => transitions;
		public StateNode Root => root;

		public event Action<StateNode> OnModuleChanged;

		[SerializeReference] private string name;
		[SerializeReference] private StateNode root;
		[SerializeReference] private List<Transition> transitions = new();
		private StateModule successTransition;
		private StateModule failureTransition;
		private StateGraph graph;
		private List<StateNode> children = new();

		public StateModule() { }

		public StateModule(string stateName) => name = stateName;

		internal Status Enter()
		{
			foreach (Transition transition in transitions)
				transition.EnterTransition();

			return StartNode(root);
		}

		internal void Update()
		{
			Status status = root.OnUpdate();

			//This is not going to work, if the state returns success or failure the transition logic never runs
			//obviously this is not ideal as you can get locked into a state that only returns success etc

			if (status is Status.Success or Status.Failure)
			{
				StateModule toState = Graph.DefaultState;
				switch (status)
				{
					case Status.Success:
						if (successTransition is not null)
							toState = successTransition;
						break;
					case Status.Failure:
						if (failureTransition is not null)
							toState = failureTransition;
						break;
				}

				graph.RequestStateChange(this, toState);

				//TODO I don't like this, returning early makes me feel like I should do it different
				return;
			}

			foreach (Transition transition in transitions)
			{
				if (transition.CanTransition())
				{
					graph.RequestStateChange(transition);
					break;
				}
			}
		}

		public void Exit()
		{
			foreach (Transition transition in transitions)
				transition.ExitTransition();

			//Do any exit logic usually to do with cleanup of states
			ExitNode(root);
		}

		internal Status StartNode(StateNode node)
		{
			Status status = node.OnEnter();
			OnModuleChanged?.Invoke(node);
			return status;
		}

		internal void ExitNode(StateNode node)
		{
			node.OnExit();
			OnModuleChanged?.Invoke(node);
		}

		public void AddTransition(StateModule to, Condition[] conditions, ConditionOperator conditionOperator)
			=> transitions.Add(new Transition(this, to, conditions, conditionOperator));

		public void AddSuccessTransition(StateModule to) => successTransition = to;

		public void AddFailureTransition(StateModule to) => failureTransition = to;

		public IEnumerable<StateNode> GetPathToActiveNode()
		{
			StateNode current = root;
			yield return current;

			while (current is IParent parent)
			{
				current = parent.GetActiveChild();
				yield return current;
			}
		}

		internal void SetRoot(StateNode rootNode)
		{
			AddChild(rootNode);
		}

		public void AddChild(StateNode child)
		{
			root = child;
			root.Parent = this;
		}

		public void AddChild(StateNode child, int index)
		{
			root = child;
			root.Parent = this;
		}

		public void RemoveChild(StateNode child)
		{
			root = null;
		}

		public StateNode GetActiveChild() => root;

		public IEnumerable<StateNode> GetChildren()
		{
			return children.Count > 0 ? children : BuildChildrenList();
		}

		private IEnumerable<StateNode> BuildChildrenList()
		{
			HashSet<IParent> parentsChecked = new();
			List<IParent> parentsToCheck = new();
			children = new() { root };

			if (root is not IParent rootParent)
				return children;

			parentsToCheck.Add(rootParent);

			while (parentsToCheck.Count > 0)
			{
				IParent currentParent = parentsToCheck[0];
				parentsChecked.Add(currentParent);

				foreach (StateNode child in currentParent.GetChildren())
				{
					children.Add(child);

					if (child is IParent parentToCheck && !parentsChecked.Contains(parentToCheck))
						parentsToCheck.Add(parentToCheck);
				}

				parentsToCheck.Remove(currentParent);
			}

			return children;
		}
	}
}