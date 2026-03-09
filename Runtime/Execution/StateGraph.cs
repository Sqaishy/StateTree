using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace StateTree
{
	[CreateAssetMenu(menuName = "State Tree/State Graph")]
	public class StateGraph : ScriptableObject
	{
		public StateModule CurrentState
		{
			get => currentState;
			internal set => currentState = value;
		}
		public StateTreeAgent Agent { get; internal set; }
		public StateModule DefaultState
		{
			get => defaultState;
			set => defaultState = value;
		}

		public TransitionSequencer Sequencer
		{
			get => sequencer;
			set => sequencer = value;
		}
		/// <summary>
		/// The source blackboard and its contents as set from the authoring graph
		/// </summary>
		internal StateBlackboard SourceBlackboard
		{
			get => sourceBlackboard;
			set => sourceBlackboard = value;
		}

		public StateBlackboard Blackboard => Agent.Blackboard;

		public event Action<StateModule, StateModule> OnStateChanged;

		[SerializeReference] public List<StateModule> nodes = new();

		private StateModule currentState;
		private TransitionSequencer sequencer;
		[SerializeReference] private StateModule defaultState;
		[FormerlySerializedAs("blackboard")] [SerializeReference] private StateBlackboard sourceBlackboard = new();

		internal void Enter()
		{
			currentState = defaultState;
			currentState.Enter();

			OnStateChanged?.Invoke(defaultState, defaultState);
		}

		internal void Update()
		{
			currentState.Update();
		}

		internal void Exit()
		{
			currentState.Exit();
		}

		internal void RequestStateChange(StateModule from, StateModule to)
		{
			if (!sequencer.RequestTransition(from, to))
				return;

			OnStateChanged?.Invoke(from, to);
		}

		internal void RequestStateChange(Transition transition)
		{
			RequestStateChange(transition.from, transition.to);
		}
	}

	public class TransitionSequencer
	{
		private StateGraph StateGraph { get; set; }

		public TransitionSequencer(StateGraph stateGraph)
		{
			StateGraph = stateGraph;
		}

		/// <returns>Whether the transition was successful</returns>
		public bool RequestTransition(StateModule from, StateModule to)
		{
			from.Exit();
			StateGraph.CurrentState = to;
			to.Enter();

			return true;
		}
	}

	public class StateGraphBuilder
	{
		private readonly StateGraph stateGraph;
		private readonly StateMachineAgent agent;
		private List<StateModule> states;

		public StateGraphBuilder(StateTreeAgent agent, StateModule defaultState)
		{
			stateGraph = ScriptableObject.CreateInstance<StateGraph>();
			stateGraph.Agent = agent;
			stateGraph.DefaultState = defaultState;
			stateGraph.Sequencer = new TransitionSequencer(stateGraph);
		}

		public StateGraphBuilder(StateGraph graphToInstantiate)
		{
			stateGraph = Object.Instantiate(graphToInstantiate);
		}

		public StateGraph Build()
		{
			SetGraphForModules(stateGraph.DefaultState);
			return stateGraph;
		}

		private void SetGraphForModules(StateModule startingModule)
		{
			HashSet<StateModule> modulesChecked = new();
			List<StateModule> modulesToCheck = new() { startingModule };
			startingModule.Graph = stateGraph;

			while (modulesToCheck.Count > 0)
			{
				StateModule currentModule = modulesToCheck[0];
				modulesChecked.Add(currentModule);

				foreach (StateNode node in currentModule.GetChildren())
					node.Module = currentModule;

				foreach (Transition transition in currentModule.Transitions)
				{
					if (modulesChecked.Contains(transition.to))
						continue;

					transition.to.Graph = stateGraph;
					transition.SetConditionAgent(stateGraph.Agent);
					modulesToCheck.Add(transition.to);
				}

				modulesToCheck.Remove(currentModule);
			}
		}
	}
}