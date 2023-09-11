﻿using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.ParseVisitors.Expressions;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private readonly Lazy<DeclarationVisitor> _declarationVisitor;
        private readonly Lazy<ExpressionVisitor> _expressionVisitor;
        private readonly Lazy<FunctionWrapper> _functionWrapper;
        private readonly Lazy<TypeHandlingWrapper> _typeHandlingWrapper;
        private readonly Lazy<GeneralEvaluator> _generalEvaluator;
        private readonly Lazy<ForVisitor> _forVisitor;
        private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;
        private readonly Lazy<IfExpressionVisitor> _ifExpressionVisitor;
        private readonly Lazy<ChoiceConversionVisitor> _choiceConversionVisitor;

        public MainVisitor(ApplicationState applicationState,
                           Lazy<DeclarationVisitor> declarationVisitor,
                           Lazy<ExpressionVisitor> expressionVisitor,
                           Lazy<FunctionWrapper> functionWrapper,
                           Lazy<TypeHandlingWrapper> typeHandlingWrapper,
                           Lazy<GeneralEvaluator> generalEvaluator,
                           Lazy<ForVisitor> forVisitor,
                           Lazy<ValueDefinitionVisitor> valueDefinitionVisitor,
                           Lazy<IfExpressionVisitor> ifExpressionVisitor,
                           Lazy<ChoiceConversionVisitor> choiceConversionVisitor) : base(applicationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _functionWrapper = functionWrapper;
            _typeHandlingWrapper = typeHandlingWrapper;
            _functionWrapper.Value.FunctionCallVisitor.FunctionRequestedExecution -= FunctionRequestedExecutionCallback;
            _functionWrapper.Value.FunctionCallVisitor.FunctionRequestedExecution += FunctionRequestedExecutionCallback;
            _generalEvaluator = generalEvaluator;
            _forVisitor = forVisitor;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            ApplicationState.CurrentScope.LookupManager.VariableBound +=
                _generalEvaluator.Value.Propagator.HandleVariableBound!;
            _ifExpressionVisitor = ifExpressionVisitor;
            _choiceConversionVisitor = choiceConversionVisitor;
            _generalEvaluator.Value.ArithmeticExpressionResolved += ArithmeticExpressionResolved;
            _generalEvaluator.Value.StringExpressionResolved += StringExpressionResolved;
            _generalEvaluator.Value.ForExpressionResolved += ForExpressionResolved;
            _generalEvaluator.Value.ExpressionWithNoValueFound += ValueLessExpressionDetected;
        }

        private void FunctionRequestedExecutionCallback(object? sender, FunctionRequestedExecutionEventArgs e)
        {
            foreach (var block in e.FunctionCall.Function.FunctionBody)
            {
                foreach (var child in block.children)
                {
                    // Ugly but due to aliases in the grammar the delegation to the value definition visitor would require
                    // writing out every possible value definition sub rule as a visitor method call in order to make this work
                    if (child is Verse.Value_definitionContext)
                    {
                        var res = VariableConverter.Convert(_valueDefinitionVisitor.Value.Visit(child)!);
                        _functionWrapper.Value.FunctionCallVisitor.OnVariableResolved(res);
                        continue;
                    }
                    child.Accept(this);
                }
            }
        }

        public override Variable VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor.Value);

            if (declaredVariable.Value.TypeData.Name != "undefined")
            {
                ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
            }
            return declaredVariable;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Value.Visit(context);
            _expressionVisitor.Value.Clean();

            _generalEvaluator.Value.ExecuteExpression(res);
            return null!;
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var res = _functionWrapper.Value.FunctionDeclarationVisitor.Visit(context);
            ApplicationState.AddFunction(res);
            return null!;
        }

        public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var result = _functionWrapper.Value.FunctionCallVisitor.Visit(context);

            if (result == null || result.IsVoid)
            {
                return null!;
            }
            if (ApplicationState.CurrentScopeLevel == 1)
            {
                Printer.PrintResult(result);
            }
            return result;
        }

        public override object VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            var novelType = _typeHandlingWrapper.Value.TypeDefinitionVisitor.Visit(context);
            this.ApplicationState.Types.Add(novelType.Name, novelType);
            return null!;
        }

        public override object VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
        {
            this._typeHandlingWrapper.Value.TypeMemberVisitor.Visit(context);
            return null!;
        }

        public override object VisitIf_block(Verse.If_blockContext context)
        {
            var result = _ifExpressionVisitor.Value.Visit(context);
            bool isSuccess = false;
            _generalEvaluator.Value.IfExpressionResolved += (sender, args) =>
            {
                isSuccess = args.IsSuccess;
            };
            _generalEvaluator.Value.ExecuteExpression(result);

            if (isSuccess)
            {
                // Add variables from If Head
                if (result.ScopedVariable != null)
                {
                    ApplicationState.CurrentScope.AddScopedVariable(result.ScopedVariable);
                }

                foreach (var element in result.ThenBlock)
                {
                    element.Accept(this);
                }
                // Remove after execution
                if (result.ScopedVariable != null)
                {
                    ApplicationState.CurrentScope.LookupManager.RemoveVariable(result.ScopedVariable);
                }
            }
            else
            {
                foreach (var element in result.ElseBlock)
                {
                    if (element.value_definition() != null)
                    {
                        var res = VariableConverter.Convert(_valueDefinitionVisitor.Value.Visit(element.value_definition())!);
                        _functionWrapper.Value.FunctionCallVisitor.OnVariableResolved(res);
                        continue;
                    }
                    element.Accept(this);
                }
            }
            return null!;
        }

        public override object VisitLambdaFunc([NotNull] Verse.LambdaFuncContext context)
        {
            var res = _functionWrapper.Value.FunctionDeclarationVisitor.Visit(context);
            ApplicationState.AddFunction(res);
            return null!;
        }

        public override object VisitFunc([NotNull] Verse.FuncContext context)
        {
            var res = _functionWrapper.Value.FunctionDeclarationVisitor.Visit(context);
            ApplicationState.AddFunction(res);
            return null!;
        }

        public override object VisitFor_rule([NotNull] Verse.For_ruleContext context)
        {
            var forExpression = _forVisitor.Value.Visit(context);
            _generalEvaluator.Value.ExecuteExpression(forExpression);
            return null!;
        }

        public override object VisitQuestionmark_operator([NotNull] Verse.Questionmark_operatorContext context)
        {
            var result = _choiceConversionVisitor.Value.Visit(context);
            return result;
        }

        #region Callbacks
        private void StringExpressionResolved(object? sender, StringExpressionResolvedEventArgs e)
        {
            if (ApplicationState.CurrentScopeLevel > 1)
            {
                _functionWrapper.Value.FunctionCallVisitor.OnResultEvaluated(e.Result);
                return;
            }

            Printer.PrintResult(e.Result);
        }

        private void ArithmeticExpressionResolved(object? sender, ArithmeticExpressionResolvedEventArgs e)
        {
            if (ApplicationState.CurrentScopeLevel > 1)
            {
                _functionWrapper.Value.FunctionCallVisitor.OnResultEvaluated(e.Result);
                return;
            }

            Printer.PrintResult(e.Result);
        }

        private void ValueLessExpressionDetected(object? sender, ExpressionWithNoValueFoundEventArgs eventArgs)
        {
            if (ApplicationState.CurrentScopeLevel > 1)
            {
                _functionWrapper.Value.FunctionCallVisitor.OnResultEvaluated(eventArgs);
                return;
            }

            Printer.PrintResult("false?");
        }

        private void ForExpressionResolved(object? sender, ForExpressionResolvedEventArgs eventArgs)
        {
            if (ApplicationState.CurrentScopeLevel > 1)
            {
                _functionWrapper.Value.FunctionCallVisitor.OnResultEvaluated(eventArgs.ForExpression);
                return;
            }

            Printer.PrintResult(eventArgs.ForExpression.Collection!);
        }
        #endregion
    }
}
