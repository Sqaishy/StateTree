using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
	[UxmlElement]
	public partial class StateBadge : VisualElement
	{
		internal string StateName
		{
			get => this.Q<Label>().text;
			set => this.Q<Label>().text = value;
		}

		private VisualElement border => this.Q<VisualElement>(null, "Border");
		private Status? currentStatus;

		public StateBadge()
		{
			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
				AssetDatabase.GUIDToAssetPath("28b8ae4ea70913042af7ef31111d8b2a"));
			visualTree.CloneTree(this);
		}

		public StateBadge(string stateName) : this()
		{
			StateName = stateName;
		}

		internal void ChangeBadgeStatus(Status newStatus)
		{
			if (currentStatus.HasValue)
				if (border.ClassListContains(GetClassNameFromStatus(currentStatus.Value)))
					border.RemoveFromClassList(GetClassNameFromStatus(currentStatus.Value));

			border.AddToClassList(GetClassNameFromStatus(newStatus));
			border.tooltip = newStatus.ToString();

			currentStatus = newStatus;
		}

		internal void SetBorder(bool enabled)
		{
			if (enabled)
				border.AddToClassList(GetClassNameFromStatus(Status.Inactive));
			else
				border.RemoveFromClassList(GetClassNameFromStatus(Status.Inactive));
		}

		private string GetClassNameFromStatus(Status status) =>
			status switch
			{
				Status.Running => "Running",
				Status.Success => "Success",
				Status.Failure => "Failure",
				Status.Interrupted => "Failure",
				Status.Inactive => "Inactive",
				_ => throw new System.NotImplementedException()
			};
	}
}