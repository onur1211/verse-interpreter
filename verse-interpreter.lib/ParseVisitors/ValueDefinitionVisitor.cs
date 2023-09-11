using Antlr4.Runtime.Misc;
using CommandLine.Text;
using System;
using System.Reflection;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.Parser.ValueDefinitionParser;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.ParseVisitors.Expressions;
using verse_interpreter.lib.ParseVisitors.Functions;
using verse_interpreter.lib.ParseVisitors.Types;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.ParseVisitors
{
	public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult?>
	{
		private readonly Lazy<TypeInferencer> _typeInferencer;
		private readonly Lazy<ExpressionVisitor> _expressionVisitor;
		private readonly Lazy<TypeConstructorVisitor> _constructorVisitor;
		private readonly Lazy<TypeMemberVisitor> _memberVisitor;
		private readonly Lazy<ExpressionValueParser> _expressionValueParser;
		private readonly Lazy<ForVisitor> _forVisitor;
		private readonly Lazy<PropertyResolver> _resolver;
		private readonly Lazy<FunctionCallVisitor> _functionVisitor;
		private readonly Lazy<ArrayVisitor> _arrayVisitor;
		private readonly Lazy<ChoiceVisitor> _choiceVisitor;
		private readonly Lazy<ChoiceConversionVisitor> _choiceConversionVisitor;
		private readonly Lazy<RangeExpressionVisitor> _rangeExpressionVisitor;

		public ValueDefinitionVisitor(ApplicationState applicationState,
									  Lazy<TypeInferencer> typeInferencer,
									  Lazy<FunctionCallVisitor> functionVisitor,
									  Lazy<ExpressionVisitor> expressionVisitor,
									  Lazy<TypeConstructorVisitor> constructorVisitor,
									  Lazy<TypeMemberVisitor> memberVisitor,
									  Lazy<ExpressionValueParser> expressionValueParser,
									  Lazy<ForVisitor> forVisitor,
									  Lazy<PropertyResolver> resolver,
									  Lazy<ArrayVisitor> arrayVisitor,
									  Lazy<ChoiceVisitor> choiceVisitor,
									  Lazy<ChoiceConversionVisitor> choiceConversionVisitor,
									  Lazy<RangeExpressionVisitor> rangeExpressionVisitor) : base(applicationState)
		{
			_typeInferencer = typeInferencer;
			_expressionVisitor = expressionVisitor;
			_constructorVisitor = constructorVisitor;
			_memberVisitor = memberVisitor;
			_expressionValueParser = expressionValueParser;
			_forVisitor = forVisitor;
			_resolver = resolver;
			_functionVisitor = functionVisitor;
			_arrayVisitor = arrayVisitor;
			_choiceVisitor = choiceVisitor;
			_choiceConversionVisitor = choiceConversionVisitor;
			_rangeExpressionVisitor = rangeExpressionVisitor;
		}
		public override DeclarationResult VisitIntValueDef([NotNull] Verse.IntValueDefContext context)
		{
			var value = context.INT();

			// Check if the value is not null
			if (value != null)
			{
				return new DeclarationResult()
				{
					LiteralValue = value.GetText(),
					TypeName = "int"
				};
			}

			return HandleValueAssignment(context);
		}

		public override DeclarationResult VisitVariableValueDef([NotNull] Verse.VariableValueDefContext context)
		{
			// Get the variable name
			var variableName = context.ID();

			// Check if the value is a ID which is a variable name
			if (variableName != null)
			{
				// Get the actual variable intance from the lookup table
				Variable variable = _resolver.Value.ResolveProperty(variableName.GetText());

				// Convert the variable back to a declaration result
				return VariableConverter.ConvertBack(variable);
			}

			return HandleValueAssignment(context);
		}

		public override DeclarationResult VisitFalseValueDef([NotNull] Verse.FalseValueDefContext context)
		{
			var value = context.NOVALUE();

			if (value != null)
			{
				return new DeclarationResult()
				{
					TypeName = "false?"
				};
			}

			return HandleValueAssignment(context);
		}

		private DeclarationResult HandleValueAssignment([NotNull] Verse.Value_definitionContext context)
		{
			// Instead of a big if, lets use the visitor to determine which kind of value definition it actually is.
			var declarationResult = context.children.First().Accept(this);

			if (declarationResult == null)
			{
				throw new NotImplementedException();
			}

			return declarationResult;
		}

		public override DeclarationResult VisitString_rule(Verse.String_ruleContext context)
		{
			DeclarationResult declarationResult = new DeclarationResult
			{
				LiteralValue = context.SEARCH_TYPE().GetText().Replace("\"", ""),
				TypeName = "string"
			};

			return _typeInferencer.Value.InferGivenType(declarationResult);
		}

		public override DeclarationResult VisitConstructor_body(Verse.Constructor_bodyContext context)
		{
			var typeInstance = context.Accept(_constructorVisitor.Value);

			DeclarationResult declarationResult = new DeclarationResult
			{
				TypeName = typeInstance.Name,
				CustomType = typeInstance
			};

			return _typeInferencer.Value.InferGivenType(declarationResult);
		}

		public override DeclarationResult? VisitExpression(Verse.ExpressionContext context)
		{
			var expression = _expressionVisitor.Value.Visit(context);

			return _expressionValueParser.Value.ParseExpression(expression);
		}

		public override DeclarationResult VisitFunction_call(Verse.Function_callContext context)
		{
			//var functionName = context.ID().GetText();
			//var parameter = _parameterParser.Value.GetCallParameters(context.param_call_item());
			var functionCallResult = _functionVisitor.Value.Visit(context);

			return _typeInferencer.Value.InferGivenType(DeclarationResultConverter.ConvertFunctionResult(functionCallResult));
		}

		public override DeclarationResult VisitType_member_access(Verse.Type_member_accessContext context)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			var result = _memberVisitor.Value.Visit(context);
			var variable = _resolver.Value.ResolveProperty(result.AbsoluteCall);

			declarationResult.TypeName = variable.Value.TypeData.Name;
			declarationResult.CollectionVariable = variable.Value.CollectionVariable;
			declarationResult.CustomType = variable.Value.CustomType;

			switch (variable.Value.TypeData.Name)
			{
				case "int":
					declarationResult.LiteralValue = variable.Value.IntValue.ToString()!;
					break;

				case "string":
					declarationResult.LiteralValue = variable.Value.StringValue;
					break;

				default: return declarationResult;
			}

			return declarationResult;
		}

		public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
		{
			return _arrayVisitor.Value.Visit(context);
		}

        public override DeclarationResult VisitRange_expression([NotNull] Verse.Range_expressionContext context)
        {
            return _rangeExpressionVisitor.Value.Visit(context);
        }

        public override DeclarationResult VisitNumericArrayIndex([NotNull] Verse.NumericArrayIndexContext context)
		{
			return _arrayVisitor.Value.Visit(context);
		}

		public override DeclarationResult VisitVariableNameArrayIndex([NotNull] Verse.VariableNameArrayIndexContext context)
		{
			return _arrayVisitor.Value.Visit(context);
		}

		public override DeclarationResult VisitFor_rule([NotNull] Verse.For_ruleContext context)
		{
			var result = _forVisitor.Value.Visit(context);

			return _expressionValueParser.Value.ParseForExpression(result);
		}

		public override DeclarationResult VisitChoice([NotNull] Verse.ChoiceContext context)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			declarationResult.ChoiceResult = _choiceVisitor.Value.Visit(context);

			return _typeInferencer.Value.InferGivenType(declarationResult);
		}

		public override DeclarationResult VisitChoiceConversion([NotNull] Verse.ChoiceConversionContext context)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			declarationResult.ChoiceResult = ChoiceConverter.Convert(_choiceConversionVisitor.Value.Visit(context));
			return declarationResult;
		}
	}
}
