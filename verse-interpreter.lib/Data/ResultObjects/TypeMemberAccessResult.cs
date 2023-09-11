namespace verse_interpreter.lib.Data.ResultObjects
{
    public class TypeMemberAccessResult
    {
        public string PropertyName { get; set; }

        public string VariableName { get; set; }

        public string? AbsoluteCall { get; set; }

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

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public TypeMemberAccessResult()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        {
        }
    }
}