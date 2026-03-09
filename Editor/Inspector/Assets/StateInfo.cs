using System.Collections.Generic;
using System.Linq;
using StateTree.Editor.Window.Search;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [UxmlElement]
    public partial class StateInfo : VisualElement
    {
        internal string Title
        {
            get => this.Q<Label>("Title").text;
            set => this.Q<Label>("Title").text = value;
        }
        internal StateBadge CurrentState
        {
            get => this.Q<StateBadge>("CurrentState");
        }
        internal string Time
        {
            set => timeLabel.text = value;
        }

        internal string Status
        {
            set => this.Q<Label>("Status").text = value;
        }
        internal string[] Transitions
        {
            get => transitions;
            set
            {
                VisualElement transitionsContainer = this.Q<VisualElement>("Transitions");

                transitionsContainer.Clear();

                foreach (string transition in value)
                    transitionsContainer.Add(new StateBadge(transition));

                transitions = value;
            }
        }

        private string[] transitions;
        private Label timeLabel;
        private VisualElement treeView => this.Q<VisualElement>("TreeView");
        private Dictionary<StateNode, TreeItem> stateTreeMap = new();

        public StateInfo()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/StateInfo.uxml");
            visualTree.CloneTree(this);

            timeLabel = this.Q<Label>("Time");
            treeView.Clear();
        }

        internal void InitializeTreeView(StateModule module)
        {
            ShowTree(module.Root);

            foreach (StateNode node in module.GetChildren())
                UpdateTreeView(node);
        }

        internal void UpdateTreeView(StateNode nodeChanged)
        {
            if (stateTreeMap.TryGetValue(nodeChanged, out TreeItem treeItem))
                treeItem.UpdateItem();
        }

        internal void ShowTree(StateNode rootNode)
        {
            treeView.Clear();

            if (!stateTreeMap.TryGetValue(rootNode, out TreeItem treeRoot))
                treeRoot = new TreeItem(rootNode);

            treeView.Add(treeRoot);
            stateTreeMap.TryAdd(rootNode, treeRoot);

            if (rootNode is not IParent parentNode)
                return;

            RecursiveTreeView(parentNode, treeRoot);
        }

        private void RecursiveTreeView(IParent parentNode, TreeItem parentItem)
        {
            foreach (StateNode child in parentNode.GetChildren())
            {
                if (!stateTreeMap.TryGetValue(child, out TreeItem childItem))
                    childItem = new TreeItem(child);

                parentItem.Add(childItem);
                stateTreeMap.TryAdd(child, childItem);

                if (child is IParent newParent)
                    RecursiveTreeView(newParent, childItem);
            }
        }
    }

    public class ParentExtensions
    {
        //TODO potentially implement this at some point having a generic IParent might be useful
        private void Recursive<T>(IParent<T> TParent, TreeNode<T> treeItem)
        {
            foreach (T child in TParent.GetChildren())
            {
                TreeNode<T> childItem = treeItem.AddChild(child);

                if (child is IParent<T> newParent)
                    Recursive(newParent, childItem);
            }
        }
    }
}
