using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
	/// <summary>
	/// Flow nodes serve as a control structure that manages the flow of the child nodes
	/// </summary>
	public abstract class Flow : StateNode, IParent
	{
		[field: SerializeReference]
		public List<StateNode> Children { get; internal set; } = new();

		protected int currentChildIndex;

		protected Status IncrementChild()
		{
			Module.ExitNode(Children[currentChildIndex]);
			currentChildIndex++;
			return StartChild(Module.StartNode(Children[currentChildIndex]));
		}

		protected abstract Status StartChild(Status childStatus);

		public void AddChild(StateNode child)
		{
			Children.Add(child);
			child.Parent = this;
		}

		public void AddChild(StateNode child, int index)
		{
			Children.Insert(index, child);
			child.Parent = this;
		}

		public void RemoveChild(StateNode child)
		{
			if (Children.Contains(child))
				Children.Remove(child);
		}

		public StateNode GetActiveChild() => Children[currentChildIndex];

		public IEnumerable<StateNode> GetChildren()
		{
			return Children;
		}

		public override string ToString()
		{
			return GetType().Name;
		}
	}
}