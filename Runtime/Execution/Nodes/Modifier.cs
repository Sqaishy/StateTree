using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
	/// <summary>
	/// Modify the behaviour of its child node
	/// </summary>
	public class Modifier : StateNode, IParent
	{
		[field: SerializeReference]
		public StateNode Child { get; internal set; }

		protected Status RestartChild()
		{
			Module.ExitNode(Child);
			return Module.StartNode(Child);
		}

		public void AddChild(StateNode child)
		{
			Child = child;
			Child.Parent = this;
		}

		public void AddChild(StateNode child, int index)
		{
			AddChild(child);
		}

		public void RemoveChild(StateNode child)
		{
			Child = null;
		}

		public StateNode GetActiveChild()
		{
			return Child;
		}

		public IEnumerable<StateNode> GetChildren()
		{
			yield return Child;
		}
	}
}