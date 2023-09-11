namespace verse_interpreter.lib.Data.ResultObjects
{
    [Serializable]
    public class ExpressionResult
    {
        public Nullable<int> IntegerValue { get; set; }

        public string ValueIdentifier { get; set; }

        public string Operator { get; set; }
        public string StringValue { get; internal set; }

        public string TypeName { get; set; }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public ExpressionResult()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        {
            IntegerValue = null;
            ValueIdentifier = string.Empty;
            Operator = string.Empty;
        }
    }
}