using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using System.Diagnostics.CodeAnalysis;

namespace verse_interpreter.lib.ParseVisitors.Unification
{
    public class EqualityVisitor
    {
        private readonly ApplicationState _state;
        private readonly TypeInferencer _inferencer;
        private readonly GeneralEvaluator _generalEvaluator;
        private readonly PropertyResolver _propertyResolver;


        public EqualityVisitor(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 GeneralEvaluator generalEvaluator,
                                 PropertyResolver propertyResolver)

        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _generalEvaluator = generalEvaluator;
            _propertyResolver = propertyResolver;
            _state.CurrentScope.LookupManager.VariableBound += _generalEvaluator.Propagator.HandleVariableBound!;
        }

        public DeclarationResult ParseEquality(Verse.DeclarationContext context)
        {
            // Check if the variable can be unified with the given value.
            // If true then do nothing.
            // If false then return false?
            var variableName = context.ID().GetText();

            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out of scope variable {nameof(variableName)}");
            }

            Variable actualVariable = _propertyResolver.ResolveProperty(variableName);

            // If the variable has no value like x:int then return null and let
            // the declaration parser assign the value.
            if (!actualVariable.HasValue())
            {
                return null!;
            }

            var valueDef = context.value_definition();
            var arrayLiteral = context.array_literal();
            var choice = context.choice_rule();
            string value = String.Empty;

            if (valueDef != null)
            {
                value = valueDef.GetText();

                // Check if the given value is a variable
                if (_state.CurrentScope.LookupManager.IsVariable(value))
                {
                    Variable valueDefVariable = _propertyResolver.ResolveProperty(value);
                    value = this.GetValueAsStringFromVariable(valueDefVariable);
                }
            }
            else if (arrayLiteral != null)
            {
                value = arrayLiteral.GetText();
            }
            else if (choice != null)
            {
                value = choice.GetText();
            }

            if (String.IsNullOrEmpty(value))
            {
                return null!;
            }

            // If the unification fails then return false?
            if (!TryUnification(actualVariable, value))
            {
                DeclarationResult declarationResult = new DeclarationResult();
                declarationResult.Name = variableName;
                declarationResult.TypeName = "false?";
                return declarationResult;
            }

            // Otherwise return nothing.
            return null!;
        }

        private bool TryUnification(Variable variable, string value)
        {
            if (String.IsNullOrEmpty(value) || variable == null)
            {
                return false;
            }

            string variableType = variable.Value.TypeData.Name;

            switch (variableType)
            {
                case "int":
                    int number;
                    bool isValid = int.TryParse(value, out number);

                    if (!isValid)
                    {
                        return false;
                    }

                    return variable.Value.IntValue!.Value.Equals(number);

                case "string":
                    return variable.Value.StringValue.Equals(value);

                case "false?":
                    return variable.Value.TypeData.Name.Equals(value);

                default:
                    return false;
            }
        }

        private string GetValueAsStringFromVariable(Variable variable)
        {
            if (variable == null)
            {
                return String.Empty;
            }

            if (!variable.HasValue())
            {
                return String.Empty;
            }

            switch (variable.Value.TypeData.Name)
            {
                case "int":
                    return variable.Value.IntValue!.Value.ToString();

                case "string":
                    return variable.Value.StringValue;

                case "false?":
                    return variable.Value.TypeData.Name;

                default:
                    return String.Empty;
            }
        }
    }
}