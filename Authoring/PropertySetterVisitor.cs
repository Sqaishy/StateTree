using Unity.Properties;

namespace StateTree.Authoring.Code
{
	/// <summary>
	/// Set the value of the property on the <typeparamref name="TOwner"/> restricting the
	/// type to <typeparamref name="T"/>
	/// </summary>
	public class PropertySetterVisitor<TOwner, T> : IPropertyVisitor
	{
		private IProperty<TOwner> Property { get; }
		private T NewValue { get; set; }

		public PropertySetterVisitor(IProperty<TOwner> property)
		{
			Property = property;
		}

		public void SetValue(ref TOwner owner, T value)
		{
			NewValue = value;
			Property.Accept(this, ref owner);
		}

		public void Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
		{
			if (typeof(TValue) != typeof(T))
				return;

			property.SetValue(ref container, (TValue)(object)NewValue);
		}
	}
}