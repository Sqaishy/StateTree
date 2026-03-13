using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StateTree.Authoring.Code
{
	/// <summary>
	/// Resolve the type when it is dropped into a field of another type
	/// </summary>
	/// <typeparam name="T">The type to resolve in the field</typeparam>
	public class FieldResolver<T> : PointerManipulator where T : Object
	{
		private Action<T> action;

		public FieldResolver(Action<T> dragPerformed)
		{
			action = dragPerformed;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<DragUpdatedEvent>(DragUpdated);
			target.RegisterCallback<DragExitedEvent>(DragPerform);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<DragUpdatedEvent>(DragUpdated);
			target.UnregisterCallback<DragExitedEvent>(DragPerform);
		}

		private void DragUpdated(DragUpdatedEvent evt)
		{
			if (DragAndDrop.objectReferences.Length == 1 && typeof(T)
				    .IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()))
			{
				DragAndDrop.AcceptDrag();
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
		}

		private void DragPerform(DragExitedEvent evt)
		{
			T check = DragAndDrop.objectReferences[0] as T;

			if (check == null)
				return;

			action?.Invoke(check);
		}
	}
}