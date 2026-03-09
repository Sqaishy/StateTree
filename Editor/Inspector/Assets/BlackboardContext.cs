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
                AssetDatabase.GUIDToAssetPath("72084613b1e788045a9a7b50caa5dc57"));
            visualTree.CloneTree(this);
        }

        internal void SetName(string contextName) => this.Q<Label>("ContextName").text = contextName;
        internal void AddProperty(SerializedProperty contextProperty) => Add(new PropertyField(contextProperty));
    }
}
