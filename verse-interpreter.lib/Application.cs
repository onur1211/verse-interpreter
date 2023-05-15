using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Lexer;
using verse_interpreter.lib.Lookup;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.exe
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
            ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener, _services.GetRequiredService<IParseTreeListener>());

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
                .AddTransient<IParseTreeListener, ParserListener>()
                .AddTransient<MainVisitor>()
                .AddTransient<TypeInferencer>()
                .AddTransient<IEvaluator<int, List<List<ExpressionResult>>>, ArithmeticEvaluator>()
                .AddTransient<IEvaluator<string, List<List<ExpressionResult>>>, StringExpressionEvaluator>()
                .BuildServiceProvider();

            return services;
        }
    }
}
