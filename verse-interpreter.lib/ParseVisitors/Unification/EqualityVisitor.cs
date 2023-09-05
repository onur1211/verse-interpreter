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
            // Get the name of the variable
            // Example: x=2 or x=y then get 'x'
            var variableName = context.ID().GetText();

            // Check if the name is a variable
            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out-of-scope variable {variableName}");
            }

            Variable actualVariable = _propertyResolver.ResolveProperty(variableName);

            if (actualVariable.Value.CollectionVariable != null) 
            {
                return null!;
            }

            string value = GetValueFromContext(context);

            // If there is no value then return null
            if (String.IsNullOrEmpty(value))
            {
                return null!;
            }

            // Try to unify the variable with the given value
            if (!TryUnification(actualVariable, value))
            {
                return new DeclarationResult
                {
                    Name = variableName,
                    TypeName = "false?"
                };
            }

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

        private string GetValueFromContext(Verse.DeclarationContext context)
        {
            if (context.value_definition() != null)
            {
                string value = context.value_definition().GetText();

                // Check if the value is a variable and if true get the value from it
                // Example: x=y then y is the value as a variable
                if (_state.CurrentScope.LookupManager.IsVariable(value))
                {
                    Variable valueDefVariable = _propertyResolver.ResolveProperty(value);
                    return GetValueAsStringFromVariable(valueDefVariable);
                }

                return value;
            }
            else if (context.array_literal() != null)
            {
                return context.array_literal().GetText();
            }
            else if (context.choice_rule() != null)
            {
                return context.choice_rule().GetText();
            }

            return String.Empty;
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