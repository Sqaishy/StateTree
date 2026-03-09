using UnityEngine;

namespace StateTree
{
    /// <summary>
    /// Action nodes are the logic of the state tree controlling the associated agent
    /// </summary>
    public abstract class ActionNode : StateNode
    {
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
