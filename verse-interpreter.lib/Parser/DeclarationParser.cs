﻿using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.ParseVisitors.Unification;

namespace verse_interpreter.lib.Parser
{
    public class DeclarationParser
    {
        private readonly ApplicationState _state;
        private readonly Lazy<TypeInferencer> _inferencer;
        private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;
        private readonly Lazy<DeclarationVisitor> _declarationVisitor;
        private readonly Lazy<EqualityVisitor> _equalityVisitor;
        private readonly GeneralEvaluator _generalEvaluator;

        public DeclarationParser(ApplicationState applicationState,
                                 Lazy<TypeInferencer> typeInferencer,
                                 Lazy<ValueDefinitionVisitor> valueDefinitionVisitor,
                                 Lazy<ExpressionValidator> validator,
                                 Lazy<DeclarationVisitor> declarationVisitor,
                                 GeneralEvaluator generalEvaluator,
                                 Lazy<EqualityVisitor> equalityVisitor)
        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _declarationVisitor = declarationVisitor;
            _generalEvaluator = generalEvaluator;
            _equalityVisitor = equalityVisitor;
            _state.CurrentScope.LookupManager.VariableBound += _generalEvaluator.Propagator.HandleVariableBound!;
        }

        public DeclarationResult ParseDeclaration(Verse.DeclarationContext context)
        {
            var assignmentOperatorKind = context.children[1].GetText();
            return assignmentOperatorKind switch
            {
                ":" => ParseBringToScopeOperator(context),
                "=" => ParseAssignValueToExistingVariable(context),
                ":=" => ParseGiveValueOperator(context),
                _ => throw new NotImplementedException(),
            };
        }

        private DeclarationResult ParseBringToScopeOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.type().GetText();

            if (!(_state.Types.ContainsKey(type) || _state.WellKnownTypes.Any(x => x.Name == type)))
            {
                throw new InvalidOperationException($"The specified type \"{type}\" does not exist!");
            }

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
            };
        }

        private DeclarationResult ParseGiveValueOperator(Verse.DeclarationContext context)
        {
            var variable = ParseValueAssignment(context);
            return _inferencer.Value.InferGivenType(variable);
        }

        private DeclarationResult ParseAssignValueToExistingVariable(Verse.DeclarationContext context)
        {
            var variableName = context.ID().GetText();

            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out of scope variable {nameof(variableName)}");
            }
            var result = ParseValueAssignment(context);
            result.Name = variableName;
            var equalityResult = _equalityVisitor.Value.ParseEquality(result);

            if (equalityResult != null)
            {
                return equalityResult;
            }


            return result;
        }

        /// <summary>
        /// Calls a <paramref type="ValueDefinitionVisitor"/> which fetches the values out of the parse tree.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DeclarationResult ParseValueAssignment(Verse.DeclarationContext context)
        {
            DeclarationResult declarationResult = _valueDefinitionVisitor.Value.Visit(context)!;
            declarationResult!.Name = context.ID().GetText();


            declarationResult = HandleExpressionAsValue(declarationResult);
            declarationResult = HandleIndexedVariable(declarationResult);
            declarationResult = HandleArray(declarationResult);

            return declarationResult;
        }

        private DeclarationResult HandleArray(DeclarationResult declarationResult)
        {
            if (!_state.CurrentScope.LookupManager.IsVariable(declarationResult.Name))
            {
                return declarationResult;
            }

            var variable = _state.CurrentScope.LookupManager.GetVariable(declarationResult.Name);
            if (variable.Value.CollectionVariable == null || declarationResult.CollectionVariable == null)
            {
                return declarationResult;
            }

            foreach (var element in declarationResult.CollectionVariable.Values)
            {
                if (variable.Value.CollectionVariable.TypeData.Name == "any")
                {
                    break;
                }
                if (element.Value.TypeData.Name != variable.Value.CollectionVariable.TypeData.Name)
                {
                    throw new Exception($"No type mixing in the collection \"{variable.Value.CollectionVariable.TypeData.Name}[]\"");
                }
            }
            return declarationResult;
        }

        private DeclarationResult HandleExpressionAsValue(DeclarationResult declarationResult)
        {
            if (declarationResult.ExpressionResults == null)
            {
                return declarationResult;
            }
            _generalEvaluator.ArithmeticExpressionResolved += (sender, args) =>
            {
                declarationResult.LiteralValue = args.Result.ResultValue.ToString();
            };
            _generalEvaluator.StringExpressionResolved += (sender, args) =>
            {
                declarationResult.LiteralValue = args.Result.Value;
            };
            _generalEvaluator.ExecuteExpression(declarationResult.ExpressionResults, declarationResult.Name);


            return declarationResult;
        }

        private DeclarationResult HandleIndexedVariable(DeclarationResult declarationResult)
        {
            if (declarationResult.IndexedVariable == null)
            {
                return declarationResult;
            }

            if (declarationResult.CollectionVariable != null && declarationResult.CollectionVariable.Values != null)
            {
                declarationResult.CollectionVariable = new VerseCollection(declarationResult.CollectionVariable.Values);
                declarationResult.TypeName = declarationResult.IndexedVariable.Value.TypeData.Name;
                return declarationResult;
            }

            if (declarationResult.IndexedVariable != null)
            {
                Variable indexedVar = declarationResult.IndexedVariable;

                if (indexedVar.Value.IntValue != null)
                {
                    declarationResult.LiteralValue = declarationResult.IndexedVariable.Value.IntValue.ToString();
                    declarationResult.TypeName = declarationResult.IndexedVariable.Value.TypeData.Name;
                    return declarationResult;
                }

                if (indexedVar.Value.StringValue != null)
                {
                    declarationResult.LiteralValue = declarationResult.IndexedVariable.Value.StringValue;
                    declarationResult.TypeName = declarationResult.IndexedVariable.Value.TypeData.Name;
                    return declarationResult;
                }

                if (indexedVar.Value.CollectionVariable != null)
                {
                    declarationResult.CollectionVariable = declarationResult.IndexedVariable.Value.CollectionVariable;
                    declarationResult.TypeName = "collection";
                    return declarationResult;
                }
            }

            return declarationResult;
        }
    }
}
