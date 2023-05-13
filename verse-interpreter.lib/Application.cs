﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lexer;
using verse_interpreter.lib.Lookup;
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

            // Lookup Test
            // Verse Code in this test: 
            // x:=5; y:=10; z:=100; x:=500;
            this.LookupManagerTEST();
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

        /// <summary>
        /// Used for testing the lookup manager. Currently only works with int?
        /// </summary>
        private void LookupManagerTEST()
        { 
            // Verse Code in this test: 
            // x:=5; y:=10; z:=100; x:=500;

            LookupManager lookupManager = new LookupManager(new LookupTable<int?>());

            DeclarationResult x = new DeclarationResult();
            x.Name = "x";
            x.TypeName = "int";
            x.Value = 5;

            DeclarationResult y = new DeclarationResult();
            y.Name = "y";
            y.TypeName = "int";
            y.Value = 10;

            DeclarationResult z = new DeclarationResult();
            z.Name = "z";
            z.TypeName = "int";
            z.Value = 100;

            lookupManager.Add(x);
            lookupManager.Add(y);
            lookupManager.Add(z);

            var xValues = lookupManager.GetVariableValue("x");

            Console.WriteLine("Lookup Table: ");

            foreach (var value in xValues)
            {
                Console.WriteLine(value);
            }

            x.Name = "x";
            x.TypeName = "int";
            x.Value = 500;

            lookupManager.Add(x);

            xValues = lookupManager.GetVariableValue("x");

            foreach (var value in xValues)
            {
                Console.WriteLine(value);
            }
        }
    }
}
