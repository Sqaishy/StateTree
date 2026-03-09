using System;
using System.Collections.Generic;
using System.Linq;
using StateTree.Editor.Window.Search;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor.Window
{
	[UxmlElement]
	public partial class ModuleViewer : VisualElement
	{
		public event Action<StateModule> OnSetDefaultModule;

		private StateModule currentModule;
		private TreeItem selectedNode;
		private ActionButton addNodeButton;
		private ActionButton removeNodeButton;
		private ActionButton defaultModuleButton;
		private VisualElement nodeTree;

		internal void ChangeModule(StateModule module)
		{
			currentModule = module;
			this.Q<Label>("ModuleName").text = module.Name;

			addNodeButton = this.Q<ActionButton>("AddNode");
			addNodeButton.clicked += AddNode;
			removeNodeButton = this.Q<ActionButton>("RemoveNode");
			removeNodeButton.clicked += RemoveNode;
			defaultModuleButton = this.Q<ActionButton>("SetDefault");
			defaultModuleButton.clicked += () => OnSetDefaultModule?.Invoke(currentModule);

			nodeTree = this.Q<VisualElement>("NodeTree");

			ShowNodeTree();
		}

		private void AddNode()
		{
			//Now I want to try the popover again now that the editor view has a panel
			SearchView searchView = new SearchView();
			Popover popover = Popover.Build(addNodeButton, searchView);
			popover.SetPlacement(PopoverPlacement.Left);
			popover.Show();

			searchView.OnItemSelected += item =>
			{
				Debug.Log($"Selected node: {item.Data}");
				AddItemToTree(item);
				popover.Dismiss();
			};
		}

		private void RemoveNode()
		{
			Debug.Log($"Removing node {selectedNode.stateNode} from parent {selectedNode.stateNode.Parent}");

			selectedNode?.stateNode.Parent.RemoveChild(selectedNode.stateNode);

			StateTreeEditorWindow.MarkGraphDirty();

			ShowNodeTree();
		}

		private void AddItemToTree(SearchView.Item item)
		{
			if (currentModule.Root == null)
			{
				Type dataType = (Type)item.Data;

				StateNode stateNode = Activator.CreateInstance(dataType) as StateNode;

				//After setting the root node clear the node tree and add the root to it
				nodeTree = this.Q<VisualElement>("NodeTree");
				nodeTree.Clear();
				TreeItem treeItem = new TreeItem(stateNode);
				treeItem.Q<VisualElement>("Node").RegisterCallback<MouseUpEvent>(HandleTreeItemMouseClick);
				nodeTree.Add(treeItem);

				SetCurrentSelectedNode(treeItem);

				currentModule.SetRoot(stateNode);
			}
			else
			{
				if (selectedNode?.stateNode is not IParent parentNode)
					return;

				Type dataType = (Type)item.Data;
				StateNode stateNode = Activator.CreateInstance(dataType) as StateNode;

				TreeItem treeItem = new TreeItem(stateNode);
				treeItem.Q<VisualElement>("Node").RegisterCallback<MouseUpEvent>(HandleTreeItemMouseClick);
				selectedNode.Add(treeItem);
				parentNode.AddChild(stateNode);
			}

			StateTreeEditorWindow.MarkGraphDirty();
		}

		private void SetCurrentSelectedNode(TreeItem treeItem)
		{
			selectedNode?.SetSelected(false);
			selectedNode = treeItem;
			selectedNode.SetSelected(true);
		}

		private void HandleTreeItemMouseClick(MouseUpEvent evt)
		{
			SetCurrentSelectedNode(((VisualElement)evt.currentTarget).parent as TreeItem);
		}

		private void ShowNodeTree()
		{
			nodeTree.Clear();

			if (currentModule.Root == null)
				return;

			TreeItem treeRoot = new TreeItem(currentModule.Root);
			treeRoot.Q<VisualElement>("Node").RegisterCallback<MouseUpEvent>(HandleTreeItemMouseClick);
			nodeTree.Add(treeRoot);

			if (currentModule.Root is not IParent parentNode)
				return;

			RecursiveTreeView(parentNode, treeRoot);
		}

		private void RecursiveTreeView(IParent parentNode, TreeItem parentItem)
		{
			foreach (StateNode child in parentNode.GetChildren())
			{
				TreeItem childItem = new TreeItem(child);
				childItem.Q<VisualElement>("Node").RegisterCallback<MouseUpEvent>(HandleTreeItemMouseClick);

				parentItem.Add(childItem);

				if (child is IParent newParent)
					RecursiveTreeView(newParent, childItem);
			}
		}
	}
}