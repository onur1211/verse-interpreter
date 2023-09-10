using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data
{
    public class DeclarationResult
    {
        public string? LiteralValue { get; set; }

        public CustomType? CustomType { get; set; }

        public VerseCollection? CollectionVariable { get; set; }

        public ChoiceResult? ChoiceResult { get; set; }

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = null!;

        public List<List<ExpressionResult>>? ExpressionResults { get; set; }

        public Variable? IndexedVariable { get; set; }

        public DeclarationResult()
        {
            TypeName = "undefined";
        }
    }
}
