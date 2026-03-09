using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor.Window.Search
{
	public class SearchView : VisualElement
	{
		public event Action<Item> OnItemSelected;

		public List<Item> Items
		{
			get => searchItems;
			set
			{
				searchItems = value;

				rootNode = new TreeNode<Item>(new Item(title));

				foreach (Item item in searchItems)
					CreateSearchTreeNode(item);

				navigationStack.Clear();
				SetCurrentSelectedNode(rootNode);
			}
		}

		private Label titleLabel;
		private ListView listView;
		private IconButton returnButton;

		private string title = "Root";
		private List<Item> searchItems;
		private TreeNode<Item> rootNode;
		private TreeNode<Item> currentNode;
		private TreeNode<Item> searchNode;
		private Stack<TreeNode<Item>> navigationStack = new();

		public SearchView()
		{
			AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
				AssetDatabase.GUIDToAssetPath("c1ab90a5ec87e114fb643e290e8291bc"))
				.CloneTree(this);

			titleLabel = this.Q<VisualElement>("Header").Q<Label>();
			listView = this.Q<ListView>("SearchResults");
			returnButton = this.Q<VisualElement>("Header").Q<IconButton>();
			returnButton.clicked += OnNavigationReturn;

			style.width = 200f;
			style.height = 300f;

			BuildNodeList();
		}

		private VisualElement BuildNodeList()
		{
			listView.makeItem = () => new SearchViewItem<Item>();
			listView.bindItem = (element, i) =>
			{
				if (element is not SearchViewItem<Item> searchViewItem)
					return;

				searchViewItem.Item = currentNode[i];

				searchViewItem.RegisterCallback<PointerUpEvent>(HandleItemSelection);
			};
			listView.unbindItem = (element, i) =>
			{
				if (element is not SearchViewItem<Item> searchViewItem)
					return;

				searchViewItem.UnregisterCallback<PointerUpEvent>(HandleItemSelection);
			};
			listView.selectionType = SelectionType.Single;
			listView.selectionChanged += OnListSelectionChanged;

			rootNode = new TreeNode<Item>(new Item(title));
			BuildItems();
			SetCurrentSelectedNode(rootNode);

			return listView;
		}

		private void OnListSelectionChanged(IEnumerable<object> listItems)
		{
			IEnumerable<object> items = listItems as object[] ?? listItems.ToArray();

			if (!items.Any())
				return;

			if (items.First() is not TreeNode<Item> treeNodeItem)
				return;

			OnItemChosen(treeNodeItem);
		}

		private IList BuildItems()
		{
			searchItems = new();
			HashSet<Type> checkedTypes = new();

			foreach (Type nodeWithAttribute in TypeCache.GetTypesWithAttribute<NodeSearchAttribute>())
			{
				if (Attribute.GetCustomAttribute(nodeWithAttribute,
					    typeof(NodeSearchAttribute)) is not NodeSearchAttribute searchAttribute)
					continue;

				checkedTypes.Add(nodeWithAttribute);

				searchItems.Add(new Item(searchAttribute.Name ?? nodeWithAttribute.Name,
					searchAttribute.Description,
					searchAttribute.Category,
					nodeWithAttribute));
			}

			foreach (Type nodeType in TypeCache.GetTypesDerivedFrom<StateNode>())
			{
				if (nodeType.IsAbstract)
					continue;

				if (checkedTypes.Contains(nodeType))
					continue;

				searchItems.Add(new Item(nodeType.Name,
					"No description set",
					"Undefined",
					nodeType));
			}

			foreach (Item item in searchItems)
				CreateSearchTreeNode(item);

			return searchItems;
		}

		private void CreateSearchTreeNode(Item item)
		{
			string[] pathSplit = item.Path.Split('/');
			TreeNode<Item> treeNodeParent = rootNode;
			string currentPath = string.Empty;

			for (int i = 0; i < pathSplit.Length; i++)
			{
				currentPath = currentPath.Length == 0 ? currentPath + pathSplit[i]
					: currentPath + "/" + pathSplit[i];

				TreeNode<Item> node = (i < pathSplit.Length - 1
					? treeNodeParent.FindChild(childItem => childItem.Path == currentPath)
					: null) ?? treeNodeParent.AddChild(new Item(currentPath));

				if (i == pathSplit.Length - 1)
					node.Value = item;
				else
					treeNodeParent = node;
			}
		}

		private void SetCurrentSelectedNode(TreeNode<Item> node)
		{
			currentNode = node;

			listView.itemsSource = currentNode.Children;
			listView.Rebuild();
			listView.ClearSelection();

			titleLabel.text = currentNode.Value.Name;

			if (navigationStack.Count == 0)
			{
				returnButton.SetEnabled(false);
				returnButton.style.visibility = Visibility.Hidden;
			}
			else
			{
				returnButton.SetEnabled(true);
				returnButton.style.visibility = Visibility.Visible;
			}
		}

		private void HandleItemSelection(PointerUpEvent evt)
		{

		}

		private void OnNavigationReturn()
		{
			if (navigationStack.Count == 0)
				return;

			if (navigationStack.TryPop(out TreeNode<Item> node))
			{
				Debug.Log($"Popping the navigation stack from {currentNode.Value.Name} returning to {node.Value.Name}");

				SetCurrentSelectedNode(node);
			}
		}

		private void OnItemChosen(TreeNode<Item> item)
		{
			if (item.Children.Count == 0)
			{
				OnItemSelected?.Invoke(item.Value);
			}
			else
			{
				navigationStack.Push(currentNode);
				SetCurrentSelectedNode(item);
			}
		}

		public struct Item : ISearchItem
		{
			public Item(string name)
			{
				Name = name;
				Description = "";
				Category = "";
				Data = null;
			}

			public Item(string name, string description, string category, object data = null)
			{
				Name = name;
				Description = description;
				Category = category;
				Data = data;
			}

			public string Name { get; }
			public string Description { get; }
			public string Category { get; }
			public object Data { get; }
			public string Path => string.IsNullOrEmpty(Category) ? Name : Category + "/" + Name;
		}
	}

	public interface ISearchItem
	{
		string Name { get; }
		string Description { get; }
	}
}