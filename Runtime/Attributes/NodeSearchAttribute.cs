using System;

namespace StateTree
{
    /// <summary>
    /// Controls how the node is shown in the editor node search window
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeSearchAttribute : Attribute
    {
        public NodeSearchAttribute(string name, string description, string category)
        {
            Name = name;
            Description = description;
            Category = category;
        }

        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
    }
}
