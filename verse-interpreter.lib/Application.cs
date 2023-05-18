using Antlr4.Runtime;
using Microsoft.Extensions.DependencyInjection;
using verse_interpreter.lib.Data.DataVisitors;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib
{
    public class Application
    {
        private IParserErrorListener _errorListener;
        private IServiceProvider _services;
        private FileReader _reader;

        public Application()
        {
            _errorListener = new ErrorListener();
            _services = null!;
            _reader = new FileReader();
        }

        public void Run(string[] args)
        {
            _services = BuildService();
            ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);

            var inputCode = _reader.ReadFileToEnd("../../../../verse-interpreter.lib/VerseTemplate.verse");
            var parseTree = generator.GenerateParseTree(inputCode);
            var mainVisitor = _services.GetRequiredService<MainVisitor>();
            mainVisitor.VisitProgram(parseTree);

            Console.ReadKey();
        }

        private IServiceProvider BuildService()
        {
            var services = new ServiceCollection()
                .AddSingleton<ApplicationState>()
                .AddSingleton<BackpropagationEventSystem>()
                .AddTransient<DeclarationVisitor>()
                .AddTransient<ExpressionVisitor>()
                .AddTransient<FunctionDeclarationVisitor>()
                .AddTransient<FunctionExecutionVisitor>()
                .AddTransient<TypeDefinitionVisitor>()
                .AddTransient<TypeConstructorVisitor>()
                .AddTransient<MainVisitor>()
                .AddTransient<TypeInferencer>()
                .AddTransient<IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>>, ArithmeticEvaluator>()
                .AddTransient<IEvaluator<string, List<List<ExpressionResult>>>, StringExpressionEvaluator>()
                .AddTransient<ExpressionValidator>()
                .AddTransient<DeclarationParser>()
                .AddTransient<TypeMemberVisitor>()
                .AddTransient<ValueDefinitionVisitor>()
                .AddTransient<EvaluatorWrapper>()
                .AddTransient<VariableVisitor>()
                .AddTransient<TypeHandlingWrapper>()
                .BuildServiceProvider();

            return services;
        }
    }
}
