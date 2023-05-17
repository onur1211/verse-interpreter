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
using verse_interpreter.lib.Wrapper;
using Z.Expressions;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private readonly TypeHandlingWrapper _typeHandlingWrapper;
        private readonly EvaluatorWrapper _baseEvaluator;

        public MainVisitor(ApplicationState applictationState,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           TypeHandlingWrapper typeHandlingWrapper,
                           EvaluatorWrapper baseEvaluator) : base(applictationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _typeHandlingWrapper = typeHandlingWrapper;
            _baseEvaluator = baseEvaluator;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.CurrentScope.AddScopedVariable(1, declaredVariable);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            PrintResult(_baseEvaluator.ArithmeticEvaluator.Evaluate(res).ResultValue.ToString()!);
            return null!;
        }

        //public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        //{
        //    var result = context.Accept(_functionDeclarationVisitor);
        //    return null!;
        //}

        // Type Related

        public override object VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            var novelType = _typeHandlingWrapper.TypeDefinitionVisitor.Visit(context);
            this.ApplicationState.Types.Add(novelType.Name, novelType);
            return null!;
        }

        public override object VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
        {
            this._typeHandlingWrapper.TypeMemberVisitor.Visit(context);
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
