using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
	[Serializable]
	public class StateBlackboard
	{
		public List<StateContext> Contexts
		{
			get => stateContexts;
			internal set => stateContexts = value;
		}

		[SerializeReference] private List<StateContext> stateContexts = new();

		public StateBlackboard()
		{

		}

		public StateBlackboard(params StateContext[] contexts)
		{
			foreach (StateContext stateContext in contexts)
				AddContext(stateContext);
		}

		public StateBlackboard(StateBlackboard blackboard)
		{
			foreach (StateContext context in blackboard.Contexts)
				AddContext(context.Clone());
		}

		public bool TryGetContext<T>(out T value) where T : StateContext
		{
			foreach (StateContext stateContext in Contexts)
			{
				if (stateContext is not T correctContext)
					continue;

				value = correctContext;
				return true;
			}

			Debug.LogError($"Could not find context for {typeof(T).Name}");

			value = null;
			return false;
		}

		public void AddContext(StateContext newContext) => Contexts.Add(newContext);
		public void RemoveContext(StateContext newContext) => Contexts.Remove(newContext);
	}
}