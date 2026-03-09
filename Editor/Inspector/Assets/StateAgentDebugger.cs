using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [UxmlElement]
    public partial class StateAgentDebugger : VisualElement, IDisposable
    {
        private StateAgentHistory history;

        public override VisualElement contentContainer => this.Q<Foldout>();

        public StateAgentDebugger()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/StateAgentDebugger.uxml");
            visualTree.CloneTree(this);

            history = this.Q<StateAgentHistory>();
        }

        internal void Start(StateTreeAgent agent)
        {
            history.Init(agent);
        }

        internal void Update()
        {
            history.Update();
        }

        public void Dispose()
        {
            if (!Application.isPlaying)
                return;

            history?.Dispose();
        }
    }
}
