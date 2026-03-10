using System;
using StateTree;
using UnityEditor;
using UnityEngine;

namespace StateTreeSystem.Authoring.Code
{
	[CreateAssetMenu(menuName = "State Tree/Authoring Graph")]
	public class AuthoringGraph : ScriptableObject
	{
		[SerializeField] private StateGraph stateGraph;

		public StateGraph StateGraph => stateGraph;

		private void Awake()
		{
			AuthoringGraph authoringGraph = this;

			string path = AssetDatabase.GetAssetPath(authoringGraph);

			if (!EditorUtility.IsPersistent(authoringGraph))
				return;

			StateGraph newGraph = CreateInstance<StateGraph>();
			newGraph.name = authoringGraph.name + "_StateTree";
			authoringGraph.stateGraph = newGraph;
			AssetDatabase.AddObjectToAsset(newGraph, authoringGraph);
			AssetDatabase.SaveAssetIfDirty(authoringGraph);
		}
	}
}