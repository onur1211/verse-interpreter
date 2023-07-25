using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Data
{
    public class DeclarationResult
    {
        public string LiteralValue { get; set; } = null!;

        public CustomType? CustomType { get; set; }

        public VerseCollection CollectionVariable { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = null!;

        public List<List<ExpressionResult>>? ExpressionResults { get; set; } = null!;

        public Variable IndexedVariable { get; set; } = null;

        public DeclarationResult()
        {
            TypeName = "undefined";
        }
    }
}
