using System;
using UnityEditor;
using UnityEngine;

namespace StateTree.Authoring.Code
{
	public abstract class AuthoringGraph : ScriptableObject
	{
		[SerializeField] private StateGraph stateGraph;

		public StateGraph StateGraph => stateGraph;

		private void OnEnable()
		{
			stateGraph = GetOrCreateStateGraph();
		}

		public abstract StateGraph CreateRuntimeGraph();
		public abstract StateBlackboard CreateRuntimeBlackboard();

		private StateGraph GetOrCreateStateGraph()
		{
			if (stateGraph != null)
				return stateGraph;

#if UNITY_EDITOR

			AuthoringGraph authoringGraph = this;

			StateGraph newGraph = CreateRuntimeGraph();

			newGraph.name = authoringGraph.name + "_StateTree";
			authoringGraph.stateGraph = newGraph;
			AssetDatabase.AddObjectToAsset(newGraph, authoringGraph);
			AssetDatabase.SaveAssetIfDirty(authoringGraph);

			return newGraph;

			#else

			return ScriptableObject.CreateInstance<StateGraph>();

			#endif
		}
	}

	/// <summary>
	/// Used to create authoring graphs through a custom editor window
	/// </summary>
	public class WindowAuthoringGraph : AuthoringGraph
	{
		public override StateGraph CreateRuntimeGraph()
		{
			throw new NotImplementedException();
		}

		public override StateBlackboard CreateRuntimeBlackboard()
		{
			throw new NotImplementedException();
		}
	}
}