namespace verse_interpreter.lib.Data.ResultObjects
{
    public class FunctionDeclarationResult
    {
        public FunctionDeclarationResult()
        {
            VariableDeclarations = new List<DeclarationResult>();
        }

        public List<DeclarationResult> VariableDeclarations { get; private set; }

        public string FunctionName { get; set; }

        public string ReturnType { get; set; }

        public int ParameterCount { get; set; }

        public string[] Parameters { get; set; }
    }
}