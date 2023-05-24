using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
    public class ProgramVisitor : AbstractVerseVisitor<object>
    {
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private FunctionWrapper _functionWrapper;
        private TypeHandlingWrapper _typeHandlingWrapper;
        private EvaluatorWrapper _baseEvaluator;
        private BackpropagationEventSystem _backPropagator;

        public ProgramVisitor(ApplicationState applictationState,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           FunctionWrapper functionWrapper,
                           TypeHandlingWrapper typeHandlingWrapper,
                           EvaluatorWrapper baseEvaluator,
                           BackpropagationEventSystem backPropagator) : base(applictationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _functionWrapper = functionWrapper;
            _typeHandlingWrapper = typeHandlingWrapper;
            _baseEvaluator = baseEvaluator;
            _backPropagator = backPropagator;
            ApplicationState.CurrentScope.LookupManager.VariableBound += _backPropagator.HandleVariableBound!;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            _expressionVisitor.Clean();
            var expression = _baseEvaluator.ArithmeticEvaluator.Evaluate(res);
            if (expression.PostponedExpression == null)
            {
                Printer.PrintResult(expression.ResultValue.ToString()!);
            }
            else
            {
                _backPropagator.AddExpression(expression);
            }
            return null!;
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var res = _functionWrapper.FunctionDeclarationVisitor.Visit(context);
            ApplicationState.CurrentScope.AddFunction(res);
            return null!;
        }

        public override object VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var functionCallItem = _functionWrapper.FunctionCallVisitor.Visit(context);
            _functionWrapper.FunctionCallPreprocessor.BuildExecutableFunction(functionCallItem);
            //ApplicationState./*SwitchContext*/(functionCallItem.Function.FunctionName);
            // current context is in function
            // do work
            // change back
            return null!;
        }

        public override object VisitBlock([NotNull] Verse.BlockContext context)
        {
            foreach (var child in context.children)
            {
                child.Accept(this);
            }

            return null!;
        }

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
    }
}
