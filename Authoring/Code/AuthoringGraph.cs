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

			AuthoringGraph authoringGraph = this;

			StateGraph newGraph = CreateRuntimeGraph();

			newGraph.name = authoringGraph.name + "_StateTree";
			authoringGraph.stateGraph = newGraph;
			AssetDatabase.AddObjectToAsset(newGraph, authoringGraph);
			AssetDatabase.SaveAssetIfDirty(authoringGraph);

			return newGraph;
		}
	}

	public abstract class CodeAuthoringGraph : AuthoringGraph
	{
		//Implement whatever else it needs to do on top
	}
}