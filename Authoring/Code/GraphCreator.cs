using System;
using StateTree;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace StateTreeSystem.Authoring.Code
{
	public static class GraphCreator
	{
		private static void CreateStateGraph()
		{
			/*AuthoringGraph graph = ScriptableObject.CreateInstance<AuthoringGraph>();
			AssetDatabase.CreateAsset(graph, "Assets/NewStateGraph.asset");

			AssetImporter importer = AssetImporter.GetAtPath("Assets/NewStateGraph.asset");

			Debug.Log($"Imported {importer.name}");*/
		}
	}

	/*[ScriptedImporter(1, "statetree")]
	public class AuthoringGraphImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			Debug.Log(ctx);
		}
	}*/
}