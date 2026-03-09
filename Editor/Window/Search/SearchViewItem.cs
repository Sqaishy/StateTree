using UnityEngine.UIElements;

namespace StateTree.Editor.Window.Search
{
	public class SearchViewItem<T> : VisualElement where T : ISearchItem
	{
		private Label nameLabel;

		public SearchViewItem()
		{
			nameLabel = new Label();
			Add(nameLabel);
		}

		public string Name { get; }
		public string Description { get; }
		public string Category { get; }

		public TreeNode<T> Item
		{
			get => userData as TreeNode<T>;
			set
			{
				userData = value;

				nameLabel.text = value.Value.Name;
				tooltip = value.Value.Description;
			}
		}
	}
}