using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [UxmlElement]
    public partial class StateAgentBlackboard : VisualElement
    {
        public override VisualElement contentContainer => this.Q<Foldout>();

        public StateAgentBlackboard()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                AssetDatabase.GUIDToAssetPath("3cb6fb35321e07649af771d35fd87fcf"));
            visualTree.CloneTree(this);
        }

        internal void CreateBlackboard(SerializedProperty blackboardProperty)
        {
            if (blackboardProperty.boxedValue == null)
                return;

            SerializedProperty contextsProperty = blackboardProperty.FindPropertyRelative(
                "stateContexts");

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            for (int i = 0; i < contextsProperty.arraySize; i++)
            {
                SerializedProperty stateContext = contextsProperty.GetArrayElementAtIndex(i);
                BlackboardContext blackboardContext = new BlackboardContext();
                blackboardContext.SetName(stateContext.boxedValue.GetType().Name);

                foreach (FieldInfo info in stateContext.boxedValue.GetType().GetFields(flags))
                    blackboardContext.AddProperty(stateContext.FindPropertyRelative(info.Name));

                Add(blackboardContext);
            }
        }
    }
}
