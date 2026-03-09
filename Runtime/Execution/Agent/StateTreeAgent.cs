using System;
using UnityEngine;

namespace StateTree
{
	public abstract class StateTreeAgent : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeReference] private StateBlackboard blackboard;

		public GameObject AgentObject => gameObject;
		public StateGraph StateGraph { get; set; }

		public StateBlackboard Blackboard
		{
			get => blackboard;
			set => blackboard = value;
		}

#if UNITY_EDITOR
		public StateHistory StateHistory { get; private set; }
#endif

		protected virtual void Awake()
		{
			#if UNITY_EDITOR

			StateHistory = new StateHistory(this);
			StateHistory.InitializeGraph();

			#endif

			StateGraph.Enter();
		}

		protected virtual void Update()
		{
			StateGraph.Update();

			#if UNITY_EDITOR

			StateHistory.Update();

			#endif
		}

		protected virtual void OnDestroy()
		{
			StateGraph.Exit();

			#if UNITY_EDITOR

			StateHistory.Dispose();

			#endif
		}

		/// <summary>
		/// Create a new instance of StateBlackboard for this agent
		/// </summary>
		protected abstract void CreateBlackboard();

		[ContextMenu("Rebuild Blackboard")]
		private void RebuildBlackboard()
		{
			StateBlackboard copy = null;

			if (Blackboard != null)
				copy = new StateBlackboard(Blackboard);

			//What I want to do is copy whatever the blackboard has before resetting it
			CreateBlackboard();
			//Then after re-creating the blackboard set its values to whatever I had copied
			//So if I set values in the inspector I don't lose them when resetting the blackboard
			//as that is very annoying when adding new state contexts

			//Loop through the newly created blackboard and check if any state contexts match
			//from the copy, if they do set the values of the blackboard to the ones from the copy

			if (copy == null)
				return;

			for (var index = 0; index < blackboard.Contexts.Count; index++)
			{
				StateContext context = blackboard.Contexts[index];

				foreach (StateContext copyContext in copy.Contexts)
				{
					if (context.GetType() != copyContext.GetType())
						continue;

					context = copyContext.Clone();
					blackboard.Contexts[index] = context;
				}
			}
		}

		public void OnBeforeSerialize()
		{

		}

		public void OnAfterDeserialize()
		{
			if (Blackboard == null)
				CreateBlackboard();

			RebuildBlackboard();
		}
	}
}