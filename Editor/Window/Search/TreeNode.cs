using System;
using System.Collections.Generic;
using System.Linq;

namespace StateTree.Editor.Window.Search
{
	public class TreeNode<T>
	{
		public T Value
		{
			get => value;
			set => this.value = value;
		}

		public TreeNode<T> Parent { get; internal set; }

		public List<TreeNode<T>> Children => children;

		private T value;
		private List<TreeNode<T>> children = new();

		public TreeNode(T value)
		{
			this.value = value;
		}

		public TreeNode<T> this[int index] => children[index];

		public TreeNode<T> AddChild(T childValue)
		{
			TreeNode<T> child = new TreeNode<T>(childValue) { Parent = this };
			children.Add(child);
			return child;
		}

		public void Traverse(Action<TreeNode<T>> action)
		{
			action(this);
			foreach (TreeNode<T> child in children)
				child.Traverse(action);
		}

		/// <summary>
		/// Finds the first child that matches the predicate
		/// </summary>
		/// <returns>
		/// The child found or null if no children match
		/// </returns>
		public TreeNode<T> FindChild(Predicate<T> predicate) =>
			children.FirstOrDefault(child => predicate(child.Value));

		public void Sort(Comparison<TreeNode<T>> comparison) => children?.Sort(comparison);
	}
}