namespace verse_interpreter.lib.Data.ResultObjects
{
    public class TypeMemberAccessResult
    {
        public string PropertyName { get; }

        public string VariableName { get; }

        public TypeMemberAccessResult(string variableName, string propertyName)
        {
            VariableName = variableName;
            PropertyName = propertyName;
        }
    }
}