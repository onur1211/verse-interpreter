using verse_interpreter.lib.Data.ResultObjects.Expressions;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class ForResult
    {
        public List<Variable> LocalVariables { get; set; }
        public ChoiceResult? Choices { get; set; }
        public List<ExpressionSet> Filters { get; set; }

        public ForResult()
        {
            LocalVariables = new List<Variable>();
            Filters = new List<ExpressionSet>();
        }
    }
}
