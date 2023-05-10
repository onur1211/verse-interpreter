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
        private ApplicationState _applicationState;
        private IServiceProvider _services;

        public Application()
        {
            _errorListener = new ErrorListener();
            _applicationState = new ApplicationState();
            _services = null!;
        }


        public void Run(string[] args)
        {
            _services = BuildService();
            ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);
            var parseTree = generator.GenerateParseTree("test(x:int, y:int):int = \n    x + y\n\nmulBySix(x:int):int = \n    x * 6\n\nmulBySix(test(1,5))");
            _services.GetRequiredService<DeclarationVisitor>().Visit(parseTree);  
        }

        private IServiceProvider BuildService()
        {
            var services = new ServiceCollection()
                .AddSingleton<ApplicationState>()
                .AddTransient<DeclarationVisitor>()
                .AddTransient<ExpressionVisitor>()
                .AddTransient<FunctionDeclarationVisitor>()
                .AddTransient<FunctionExecutionVisitor>()
                .BuildServiceProvider() ;

            return services;
        }
    }
}
