using System;
using StateTree;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DragAndDrop = UnityEditor.DragAndDrop;
using Object = UnityEngine.Object;

namespace StateTreeSystem.Authoring.Code
{
	public class TestGraphSetter : MonoBehaviour
	{
		[SerializeField] private StateGraph stateGraph;
	}

	[CustomEditor(typeof(TestGraphSetter))]
	public class GraphSetterEditor : Editor
	{
		private TestGraphSetter targetObject;
		private TestVisitor visitor = new();
		private ObjectField graphField;
		private IProperty<TestGraphSetter> graphProperty;

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
					graphProperty = property;
			}

			graphField = new ObjectField(graphProperty.Name)
			{
				objectType = typeof(StateGraph),
			};

			if (graphProperty.GetValue(ref targetObject) != null)
				graphField.SetValueWithoutNotify((StateGraph)graphProperty.GetValue(ref targetObject));

			graphField.RegisterValueChangedCallback(ValueChanged);

			graphField.RegisterCallback<DragUpdatedEvent>(DragUpdated);
			graphField.RegisterCallback<DragExitedEvent>(DragPerform);

			root.Add(graphField);

			serializedObject.ApplyModifiedProperties();

			return root;
		}

		private void ValueChanged(ChangeEvent<Object> evt)
		{
			visitor.NewStateGraph = evt.newValue as StateGraph;

			PropertyContainer.Accept(visitor, ref targetObject);

			EditorUtility.SetDirty(targetObject);
		}

		private void DragUpdated(DragUpdatedEvent evt)
		{
			if (DragAndDrop.objectReferences.Length == 1 && typeof(AuthoringGraph)
				    .IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()))
			{
				DragAndDrop.AcceptDrag();
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
		}

		private void DragPerform(DragExitedEvent evt)
		{
			AuthoringGraph graph = DragAndDrop.objectReferences[0] as AuthoringGraph;

			if (graph == null)
				return;

			graphField.value = graph.StateGraph;
		}
	}

	public class TestVisitor : IPropertyBagVisitor, IPropertyVisitor
	{
		public StateGraph NewStateGraph { get; set;}

		public void Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
		{
			foreach (IProperty<TContainer> property in properties.GetProperties(ref container))
			{
				property.Accept(this, ref container);
			}
		}

		public void Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
		{
			if (typeof(TValue) != typeof(StateGraph))
				return;

			property.SetValue(ref container, (TValue)(object)NewStateGraph);
		}
	}
}