using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Grammar;
using Z.Expressions;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private FunctionDeclarationVisitor _functionDeclarationVisitor;
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private IEvaluator<int, List<List<ExpressionResult>>> _evaluator;

        public MainVisitor(ApplicationState applictationState,
                           FunctionDeclarationVisitor functionDeclarationVisitor,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           IEvaluator<int, List<List<ExpressionResult>>> evaluator) : base(applictationState)
        {
            _functionDeclarationVisitor = functionDeclarationVisitor;
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _evaluator = evaluator;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var res = context.Accept(_declarationVisitor);
            ApplicationState.Scopes[1].AddScopedVariable(1, res);
            return base.VisitChildren(context);
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var result = context.Accept(_functionDeclarationVisitor);
            return base.VisitFunction_definition(context);
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            _expressionVisitor.ExpressionParsedSucessfully += (sender, args) =>
            {
                // The first printed result is the correct one.
                this.PrintResult(_evaluator.Evaluate(args.Expressions).ToString());
            };

            _expressionVisitor.Visit(context);
            return 1;
        }

        private void PrintResult(string result)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VERSE CODE RESULT: ");
            Console.ResetColor();
            Console.WriteLine(result);
        }
    }
}
