using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [CustomEditor(typeof(StateGraph))]
    public class StateGraphEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            StateGraph stateGraph = target as StateGraph;

            if (!stateGraph)
            {
                root.Add(new Label("Unable to cast target object to State Graph"));
                return root;
            }

            root.Add(new Label("State Graph Nodes"));
            root.Add(new Label($"Default state -> {stateGraph.DefaultState?.Name}"));

            foreach (StateModule node in stateGraph.nodes)
                root.Add(new Label(node.Name));

            Debug.Log($"Total blackboard contexts {stateGraph.SourceBlackboard.Contexts.Count}");

            foreach (StateContext context in stateGraph.SourceBlackboard.Contexts)
                root.Add(new Label($"Blackboard Context -> {context.GetType().Name}"));

            return root;
        }
    }
}
