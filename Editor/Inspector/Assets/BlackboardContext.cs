using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    public class BlackboardContext : VisualElement
    {
        public override VisualElement contentContainer => this.Q<VisualElement>("Content");

        public BlackboardContext()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/BlackboardContext.uxml");
            visualTree.CloneTree(this);
        }

        internal void SetName(string contextName) => this.Q<Label>("ContextName").text = contextName;
        internal void AddProperty(SerializedProperty contextProperty) => Add(new PropertyField(contextProperty));
    }
}
