using Antlr4.Runtime.Misc;
using CommandLine.Text;
using System;
using System.Reflection;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.Parser.ValueDefinitionParser;
using verse_interpreter.lib.ParseVisitors.Choice;
using verse_interpreter.lib.ParseVisitors.Expressions;
using verse_interpreter.lib.ParseVisitors.Functions;
using verse_interpreter.lib.ParseVisitors.Types;

namespace verse_interpreter.lib.ParseVisitors
{
	public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult?>
	{
		private readonly TypeInferencer _typeInferencer;
		private readonly ExpressionVisitor _expressionVisitor;
		private readonly TypeConstructorVisitor _constructorVisitor;
		private readonly Lazy<TypeMemberVisitor> _memberVisitor;
		private readonly ExpressionValueParser _expressionValueParser;
		private readonly Lazy<ForVisitor> _forVisitor;
		private readonly PropertyResolver _resolver;
		private readonly Lazy<FunctionCallVisitor> _functionCallVisitor;
		private readonly ArrayVisitor _arrayVisitor;


		public ValueDefinitionVisitor(ApplicationState applicationState,
									  TypeInferencer typeInferencer,
									  Lazy<FunctionCallVisitor> functionVisitor,
									  ExpressionVisitor expressionVisitor,
									  TypeConstructorVisitor constructorVisitor,
									  Lazy<TypeMemberVisitor> memberVisitor,
									  ExpressionValueParser expressionValueParser,
									  Lazy<ForVisitor> forVisitor,
									  PropertyResolver resolver,
									  ArrayVisitor arrayVisitor) : base(applicationState)
		{
			_typeInferencer = typeInferencer;
			_expressionVisitor = expressionVisitor;
			_constructorVisitor = constructorVisitor;
			_memberVisitor = memberVisitor;
			_expressionValueParser = expressionValueParser;
			_forVisitor = forVisitor;
			_resolver = resolver;
			_functionCallVisitor = functionVisitor;
			_arrayVisitor = arrayVisitor;
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
				Variable variable = _resolver.ResolveProperty(variableName.GetText());

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

			return _typeInferencer.InferGivenType(declarationResult);
		}

		public override DeclarationResult VisitConstructor_body(Verse.Constructor_bodyContext context)
		{
			var typeInstance = context.Accept(_constructorVisitor);

			DeclarationResult declarationResult = new DeclarationResult
			{
				TypeName = typeInstance.Name,
				CustomType = typeInstance
			};

			return _typeInferencer.InferGivenType(declarationResult);
		}

		public override DeclarationResult? VisitExpression(Verse.ExpressionContext context)
		{
			var expression = _expressionVisitor.Visit(context);

			return _expressionValueParser.ParseExpression(expression);
		}

		public override DeclarationResult VisitFunction_call(Verse.Function_callContext context)
		{
			var functionCallResult = _functionCallVisitor.Value.Visit(context);

			return _typeInferencer.InferGivenType(DeclarationResultConverter.ConvertFunctionResult(functionCallResult));
		}

		public override DeclarationResult VisitType_member_access(Verse.Type_member_accessContext context)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			var result = _memberVisitor.Value.Visit(context);
			var variable = _resolver.ResolveProperty(result.AbsoluteCall);

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
			return _arrayVisitor.Visit(context);
		}

		public override DeclarationResult VisitNumericArrayIndex([NotNull] Verse.NumericArrayIndexContext context)
		{
			return _arrayVisitor.Visit(context);
		}

		public override DeclarationResult VisitVariableNameArrayIndex([NotNull] Verse.VariableNameArrayIndexContext context)
		{
			return _arrayVisitor.Visit(context);
		}

		public override DeclarationResult VisitFor_rule([NotNull] Verse.For_ruleContext context)
		{
			var result = _forVisitor.Value.Visit(context);

			return _expressionValueParser.ParseForExpression(result);
		}
	}
}
