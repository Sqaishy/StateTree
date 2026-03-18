using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace StateTree.Authoring.Code
{
	public class TestGraphSetter : MonoBehaviour
	{
		[SerializeField] private StateGraph stateGraph;
	}

#if UNITY_EDITOR

	[CustomEditor(typeof(TestGraphSetter))]
	public class GraphSetterEditor : Editor
	{
		private TestGraphSetter targetObject;
		private PropertySetterVisitor<TestGraphSetter, StateGraph> visitor;
		private ObjectField graphField;
		private IProperty<TestGraphSetter> stateGraphProperty;

		private const string GraphPropertyName = "stateGraph";

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();

			IPropertyBag<TestGraphSetter> propertyBag = PropertyBag.GetPropertyBag<TestGraphSetter>();

			targetObject = serializedObject.targetObject as TestGraphSetter;

			if (!targetObject)
				return root;

			foreach (IProperty<TestGraphSetter> property in propertyBag.GetProperties(ref targetObject))
			{
				if (property.Name == GraphPropertyName)
					stateGraphProperty = property;
			}

			visitor = new(stateGraphProperty);

			graphField = new ObjectField(stateGraphProperty.Name)
			{
				objectType = typeof(StateGraph),
			};

			if (stateGraphProperty.GetValue(ref targetObject) != null)
				graphField.SetValueWithoutNotify((StateGraph)stateGraphProperty.GetValue(ref targetObject));

			graphField.AddManipulator(new FieldResolver<AuthoringGraph>(graph =>
			{
				graphField.value = graph.StateGraph;
			}));
			graphField.RegisterValueChangedCallback(ValueChanged);

			root.Add(graphField);

			serializedObject.ApplyModifiedProperties();

			return root;
		}

		private void ValueChanged(ChangeEvent<Object> evt)
		{
			visitor.SetValue(ref targetObject, (StateGraph)evt.newValue);

			EditorUtility.SetDirty(targetObject);
		}
	}

	#endif
}