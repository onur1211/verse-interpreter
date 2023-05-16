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
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using Z.Expressions;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private FunctionDeclarationVisitor _functionDeclarationVisitor;
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private TypeDefinitionVisitor _typeDefinitionVisitor;
        private IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> _arithmeticEvaluator;

        public MainVisitor(ApplicationState applictationState,
                           FunctionDeclarationVisitor functionDeclarationVisitor,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           TypeDefinitionVisitor typeDefinitionVisitor,
                           IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> arithmeticEvaluator) : base(applictationState)
        {
            _functionDeclarationVisitor = functionDeclarationVisitor;
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _typeDefinitionVisitor = typeDefinitionVisitor;
            _arithmeticEvaluator = arithmeticEvaluator;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.CurrentScope.AddScopedVariable(1, declaredVariable);
            return null!;
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var result = context.Accept(_functionDeclarationVisitor);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            _expressionVisitor.Clean();
            PrintResult(_arithmeticEvaluator.Evaluate(res).ResultValue.ToString());
            return null!;
        }

        public override object VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            var novelType = _typeDefinitionVisitor.Visit(context);
            this.ApplicationState.Types.Add(novelType.Name, novelType);
            return null!;
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
