namespace verse_interpreter.lib.Data.ResultObjects
{
	public class TypeMemberAccessResult
	{
		public string PropertyName { get; set; } = null!;

		public string VariableName { get; set; } = null!;

		public string AbsoluteCall { get; set; } = null!;

		public TypeMemberAccessResult? ChildResult { get; set; }

		public TypeMemberAccessResult(string variableName, string propertyName)
		{
			VariableName = variableName;
			PropertyName = propertyName;
		}

		public TypeMemberAccessResult(string variableName, string propertyName, TypeMemberAccessResult child)
		{
			VariableName = variableName;
			PropertyName = propertyName;
			ChildResult = child;
		}

		public TypeMemberAccessResult()
		{
		}
	}
}