using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib;
using verse_interpreter.lib.Lexer;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.exe
{
    public class Application
    {
        private IParserErrorListener _errorListener;
        private IParseTreeListener _parseTreeListener;
        private ApplicationState _applicationState;
        private IServiceProvider _services;

        public Application()
        {
            _errorListener = new ErrorListener();
            _parseTreeListener = new ParserListener();
            _applicationState = new ApplicationState();
            _services = null!;
        }

        public void Run(string[] args)
        {
            _services = BuildService();
            ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener, _parseTreeListener);
            var parseTree = generator.GenerateParseTree("x:int; y:int");
            var mainListener = _services.GetRequiredService<MainVisitor>();
            mainListener.VisitProgram(parseTree);
            // Note: The most top level element --> such as a function_declaration has differnet visitors as children and according to that traverses the tree in a scoped manner.
        }

        private IServiceProvider BuildService()
        {
            var services = new ServiceCollection()
                .AddSingleton<ApplicationState>()
                .AddTransient<DeclarationVisitor>()
                .AddTransient<ExpressionVisitor>()
                .AddTransient<FunctionDeclarationVisitor>()
                .AddTransient<FunctionExecutionVisitor>()
                .AddTransient<MainVisitor>()
                .BuildServiceProvider();

            return services;
        }
    }
}
