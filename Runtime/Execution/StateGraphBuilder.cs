using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
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
					transition.SetConditionAgent(stateGraph.Agent);

					if (modulesChecked.Contains(transition.to))
						continue;

					transition.to.Graph = stateGraph;
					modulesToCheck.Add(transition.to);
				}

				if (currentModule.SuccessTransition != null)
				{
					if (!modulesChecked.Contains(currentModule.SuccessTransition)
					    && !modulesToCheck.Contains(currentModule.SuccessTransition))
						modulesToCheck.Add(currentModule.SuccessTransition);
				}

				if (currentModule.FailureTransition != null)
				{
					if (!modulesChecked.Contains(currentModule.FailureTransition)
					    && !modulesToCheck.Contains(currentModule.FailureTransition))
						modulesToCheck.Add(currentModule.FailureTransition);
				}

				modulesToCheck.Remove(currentModule);
			}
		}
	}
}