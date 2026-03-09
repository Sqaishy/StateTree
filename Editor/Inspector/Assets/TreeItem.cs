using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [UxmlElement]
    public partial class TreeItem : VisualElement
    {
        public override VisualElement contentContainer => this.Q<VisualElement>("Content");

        internal StateNode stateNode;

        public TreeItem()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/TreeItem.uxml");
            visualTree.CloneTree(this);
        }

        internal TreeItem(StateNode node) : this()
        {
            stateNode = node;

            VisualElement container = this.Q<VisualElement>("Node");
            container.Clear();

            container.Add(new StateBadge(stateNode.ToString()));
        }

        internal void UpdateItem()
        {
            this.Q<VisualElement>("Node").Q<StateBadge>().ChangeBadgeStatus(stateNode.CurrentStatus);
        }

        internal void SetSelected(bool selected) =>
            this.Q<VisualElement>("Node").Q<StateBadge>().SetBorder(selected);
    }
}
