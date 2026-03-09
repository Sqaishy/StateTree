using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
    public interface IParent
    {
        /// <summary>
        /// Add a child to the parent
        /// </summary>
        /// <param name="child">The child to add</param>
        void AddChild(StateNode child);
        /// <summary>
        /// Add a child to the parent at the index
        /// </summary>
        /// <param name="child">The child to add</param>
        /// <param name="index">The index to insert the child at</param>
        void AddChild(StateNode child, int index);
        /// <summary>
        /// Removes a child from the parent
        /// </summary>
        /// <param name="child">The child to remove</param>
        void RemoveChild(StateNode child);
        /// <returns>Gets the active child the parent is processing</returns>
        StateNode GetActiveChild();
        /// <returns>Gets all the children from the parent</returns>
        IEnumerable<StateNode> GetChildren();
    }

    public interface IParent<T>
    {
        void AddChild(T child);
        void AddChild(T child, int index);
        void RemoveChild(T child);
        T GetActiveChild();
        IEnumerable<T> GetChildren();
    }
}
