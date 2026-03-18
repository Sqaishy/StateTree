using System;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateTree.Authoring.Code
{
	#if UNITY_EDITOR
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
#if UNITY_EDITOR

			target.RegisterCallback<DragUpdatedEvent>(DragUpdated);
			target.RegisterCallback<DragExitedEvent>(DragPerform);

#endif
		}

		protected override void UnregisterCallbacksFromTarget()
		{
#if UNITY_EDITOR

			target.UnregisterCallback<DragUpdatedEvent>(DragUpdated);
			target.UnregisterCallback<DragExitedEvent>(DragPerform);

#endif
		}

		private void DragUpdated(DragUpdatedEvent evt)
		{
#if UNITY_EDITOR

			if (DragAndDrop.objectReferences.Length == 1 && typeof(T)
				    .IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()))
			{
				DragAndDrop.AcceptDrag();
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}

#endif
		}

		private void DragPerform(DragExitedEvent evt)
		{
#if UNITY_EDITOR

			T check = DragAndDrop.objectReferences[0] as T;

			if (check == null)
				return;

			action?.Invoke(check);

#endif
		}
	}

#endif
}