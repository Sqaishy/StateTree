using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
}