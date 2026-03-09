using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [CustomEditor(typeof(StateTreeAgent), true)]
    public class StateMachineAgentEditor : UnityEditor.Editor
    {
        private SerializedProperty blackboardProperty;
        private VisualTreeAsset inspectorAsset;
        private StateAgentDebugger debugger;
        private Foldout debuggerFoldout;

        private void OnEnable()
        {
            blackboardProperty = serializedObject.FindProperty("blackboard");

            inspectorAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/StateAgentInspector.uxml");

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;

            debugger?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            inspectorAsset.CloneTree(root);

            /*if (!serializedObject.FindProperty("stateGraph").objectReferenceValue)
                return root;*/

            debugger = root.Q<StateAgentDebugger>();
            debuggerFoldout = debugger.Q<Foldout>();
            debugger.Start(target as StateTreeAgent);

            StateAgentBlackboard blackboard = root.Q<StateAgentBlackboard>();
            blackboard.CreateBlackboard(blackboardProperty);

            return root;
        }

        private void Update()
        {
            if (!Application.isPlaying || EditorApplication.isPaused)
                return;

            if (!debuggerFoldout.value)
                return;

            debugger.Update();
        }
    }
}
