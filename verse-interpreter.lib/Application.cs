using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
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
            Test();
            Test1<int>("test");
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
                .AddTransient<TypeHandlingWrapper>()
                .BuildServiceProvider();

            return services;
        }

        Dictionary<Type, object> keyValuePairs = new Dictionary<Type, object>();

        private void Test()
        {
            keyValuePairs.Add(typeof(int), new Dictionary<string, int>());
            var res = (Dictionary<string, int>)keyValuePairs[typeof(int)];
            res.Add("test", 25);
        }

        private void Test1<T>(string variableName)
        {

        }
    }
}
