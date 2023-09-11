namespace verse_interpreter.lib.Data.Variables.Utility
{
	public static class VariableUtility
	{
		public static ValueObject Copy(this ValueObject value)
		{
			ValueObject valueObject = new ValueObject(value.TypeData.Name);
			valueObject.StringValue = value.StringValue;
			valueObject.IntValue = value.IntValue;
			//valueObject.CollectionVariable ??= new VerseCollection(value.CollectionVariable.Values.DeepClone());

			return valueObject;
		}
	}
}
